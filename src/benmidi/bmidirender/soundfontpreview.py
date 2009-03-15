"""
SoundFontPreview
Ben Fisher, 2009, GPL
halfhourhacks.blogspot.com
"""

from Tkinter import *
import tkSimpleDialog
import soundfontpreview_get
import midirender_util
import midirender_runtimidity
import os

sfubardir = 'soundfontpreview' #directory to look for sfubar.exe and sample mids
timiditydir = 'timidity' #directory of timidity program
labelFieldNames = ['name', 'date', 'author', 'copyright', 'comment']
def pack(o, **kwargs): o.pack(**kwargs); return o
class BSoundFontPreview():
	def setLabels(self, objInfo):
		for name in labelFieldNames:
			att = getattr(objInfo, name)
			self.lblInfoFields[name]['text'] = name+': '+ (att if att else 'None')
	
	def autoSet(self):
		if self.objSf.type=='pat':
			self.varPrevVoiceOnly.set(1)
			self.lblMidi['text'] = 'scale.mid'
		else:
			if len(self.objSf.presets)>50: #most likely a GM set
				self.varPrevVoiceOnly.set(0)
				self.lblMidi['text'] = 'bossa.mid'
			else:
				self.varPrevVoiceOnly.set(1)
				if (len(self.objSf.presets)<4) and self.objSf.presets[0].presetNumber==0: #most likely a piano instrument
					self.lblMidi['text'] = 'grieg.mid'
				else:
					self.lblMidi['text'] = 'scale.mid'
				
	
	def __init__(self, root):
		root.title('SoundFont Information')
		
		frameMain = pack( Frame(root), side=TOP,fill=BOTH, expand=True)
		
		
		frameLoadSf = Frame(frameMain)
		Label(frameLoadSf, text='Soundfont').pack(side=LEFT)
		self.txtSf = StringVar()
		pack(Entry(frameLoadSf, textvariable=self.txtSf ), side=LEFT,padx=5)
		
		Button(frameLoadSf,text='Load',command=self.loadSfDialog).pack(side=LEFT,padx=5)
		frameLoadSf.grid(row=0, column=0, columnspan=2)
		
		frameInfo = Frame(frameMain)
		self.lblInfoFields = {}
		for name in labelFieldNames:
			self.lblInfoFields[name] = pack(Label(frameInfo, text=name+': '),side=TOP,anchor='nw')
		frameInfo.grid(row=1, column=0)
		
		frameVoices = Frame(frameMain)
		Label(frameVoices, text='Voices').pack(side=TOP)
		self.lbVoices = pack(midirender_util.ScrolledListbox(frameVoices, width=45, height=9), side=TOP)
		frameVoices.grid(row=1, column=1)
		
		framePreview = Frame(frameMain)
		tupfontbtn = ('Verdana', 14, 'normal')
		Button(framePreview,text='Preview Soundfont', font=tupfontbtn, command=self.previewSf).pack()
		
		frameMidi = Frame(framePreview)
		self.lblMidi = pack(Label(frameMidi, text='scale.mid'), side=LEFT,fill=BOTH)
		Button(frameMidi,text='..',command=self.loadMid).pack(side=LEFT,padx=45)
		frameMidi.pack(side=TOP, anchor='w',pady=35)
		self.varPrevVoiceOnly = IntVar(); self.varPrevVoiceOnly.set(0)
		Checkbutton(framePreview, var=self.varPrevVoiceOnly, text='Preview only selected voice.').pack(side=TOP, anchor='w',pady=1)
		framePreview.grid(row=2, column=0, columnspan=2)
		
		frameMain.grid_columnconfigure(0, weight=1, minsize=20)
		frameMain.grid_columnconfigure(1, weight=1, minsize=20)
		frameMain.grid_rowconfigure(1, weight=1, minsize=20)
		
		
		self.objSf = None
	
		
	
	def loadSfDialog(self):
		
		if not self.txtSf.get() or (self.objSf and self.txtSf.get() == self.objSf.filename):
			filename = midirender_util.ask_openfile(title="Open SoundFont", types=['.sf2|SoundFont','.pat|Patch','.sbk|SoundFont1'])
			if not filename: return
		else:
			filename = self.txtSf.get()
			if not os.path.exists(filename): midirender_util.alert("Couldn't find file."); return
		self.loadSf(filename)
		
	#other scripts can call this.
	def loadSf(self,filename):
		self.txtSf.set(filename)
		if filename.lower().endswith('.pat'):
			#gus (gravis ultrasound) patch
			namestart, nameend = os.path.split(filename)
			self.lbVoices.delete(0, END)
			self.lbVoices.insert(END, nameend)
			
			self.objSf = soundfontpreview_get.SoundFontInfo()
			self.objSf.type='pat'
			self.objSf.name = nameend
			self.objSf.filename = filename
		else:
			
			
			try:
				objSf = soundfontpreview_get.getpresets(filename, sfubardir)
			except soundfontpreview_get.SFInfoException, e:
				midirender_util.alert(str(e))
				return
				
			self.setLabels(objSf)
			self.lbVoices.delete(0, END)
			for preset in objSf.presets:
				self.lbVoices.insert(END, str(preset))
		
			self.objSf = objSf
			self.objSf.filename = filename
		
		self.autoSet()
		
	
	def loadMid(self):
		filename = midirender_util.ask_openfile(title="Choose midi song", types=['.mid|Midi'], initialfolder=sfubardir)
		if not filename: return
		self.lblMidi['text'] = filename
		
	def previewSf(self):
		if not self.objSf: return
		
		#check for mid, see if it exists
		curMid = self.lblMidi['text']
		if not os.path.exists(curMid):
			curMid = os.path.join(sfubardir, curMid)
		if not os.path.exists(curMid):
			midirender_util.alert("Couldn't find midi file.")
			return
		
		#get cfg data
		escapedfname = self.objSf.filename.replace('"','\\"')
		if self.objSf.type!='pat':
			sel = self.lbVoices.curselection()
			if self.varPrevVoiceOnly.get() and len(sel)>0:
				#need to map that voice to bank0, program0
				voiceIndex = int(sel[0])
				
				voiceData = self.objSf.presets[voiceIndex]
				try:
					bank = int(voiceData.bank)
					program = int(voiceData.presetNumber)
				except ValueError:
					midirender_util.alert("Couldn't parse bank/preset number.")
					return
				
				strCfg = '\nbank 0\n'
				strCfg += '000 %font "'+escapedfname+'" %d %d'%(bank, program) + '\n'
			else:
				strCfg = '\n'
				strCfg += 'soundfont "'+escapedfname+'"\n'
				
		else:
			strCfg = '\nbank 0\n'
			strCfg += '000 "'+escapedfname+'" \n'
		
		runInThread = True
		try:
			midirender_runtimidity.runTimidity(strCfg, curMid, timiditydir, runInThread)
		except midirender_runtimidity.RunTimidityException, e:
			midirender_util.alert("Error:"+str(e))




#Simple dialog. returns a SoundFontInfoPreset object
class ChooseSoundFontPresetDialog(tkSimpleDialog.Dialog): 
	result = None
	def __init__(self, top, soundfontFilepath, default=0, title='Choose SoundFont Voice:'):
		self.soundfontFilepath = soundfontFilepath
		self.defaultInst = default
		tkSimpleDialog.Dialog.__init__(self, top, title)
		
	def body(self, top):
		frTop = Frame(top)
		frTop.grid(row=0, column=0, columnspan=2)
		
		Label(frTop, text="Choose SoundFont Voice:").pack(side=TOP)
		
		self.lbPresetChoose = midirender_util.ScrolledListbox(frTop, selectmode=SINGLE, width=45, height=15)
		self.lbPresetChoose.pack(side=TOP)
		
		#fill 
		try:
			objSf = soundfontpreview_get.getpresets(self.soundfontFilepath, sfubardir)
		except soundfontpreview_get.SFInfoException, e:
			midirender_util.alert(str(e))
			return
			
		self.lbPresetChoose.delete(0, END)
		for preset in objSf.presets:
			self.lbPresetChoose.insert(END, str(preset))
	
		self.objSf = objSf
		
		self.lbPresetChoose.selection_set(self.defaultInst)
		self.lbPresetChoose.see(self.defaultInst)
		
		self.bind('<MouseWheel>',self.scroll) #binding for Windows
		self.bind('<Button-4>',self.scroll) #binding for Linux
		self.bind('<Button-5>',self.scroll)
	
		return None # initial focus
		
	def apply(self):
		sel = self.lbPresetChoose.curselection() #returns a tuple of selected items
		if len(sel)==0: 
			self.result = None
		else:
			index = int(sel[0])
			self.result = self.objSf.presets[index]
		
	def scroll(self, event):
		if event.num == 5 or event.delta == -120:
			self.lbPresetChoose.yview_scroll(5, 'units')
		if event.num == 4 or event.delta == 120:
			self.lbPresetChoose.yview_scroll(-5, 'units')
		
if __name__=='__main__':
	if True:
		def start(top):
			def callback():
				print 'hi'
				dlg = ChooseSoundFontPresetDialog(top, r'C:\Projects\midi\!midi_to_wave\soundfonts_good\sf2_other\vintage_dreams_waves_v2.sf2')
				print dlg.result
				
			Button(text='go', command=callback).pack()
			

		root = Tk()
		start(root)
		root.mainloop()
		

	else:
		root = Tk()
		app = BSoundFontPreview(root)
		root.mainloop()

