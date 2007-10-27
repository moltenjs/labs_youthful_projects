
// Please feel free to edit this file!
// This file should be read by JavaScript without any processing,
// and Python with a little processing (// to #). Comments must be on their own line.

// The number of tabs can be from 1 to 8, so add tabs as you like.


// ModeChar=Mode Name	Hotkey
modes = [
'U=Upper mode	Control+Up',
'H=Hat		Control+6',
'L=Grave Accent	Control+L',
'R=Acute Accent	Control+R',
'D=Diaeresis	Control+D',
'T=Tilde		Control+T' ,
'=Normal		Control+ ' ]

//Hotkey	Unicode value		Description (optional)
keys = [
'Alt+Shift+4	128		Euro',
'Alt+,			130		Low curved quote',
'Alt+F		131		Script f (function)',
'Alt+Shift+,		132		Low double curved quotes',
'Alt+Shift+.		133		Ellipsis',
'Alt+T		134		Dagger (typography)',
'Alt+Shift+T	135		Double Dagger (typography)',
'Alt+Shift+6	136		Circumflex (diacritic hat)',
'Alt+Shift+5	137		Permille (per 1000 instead of per 100)',
'H/Shift+S		138		S with inverted circumflex',
'Alt+[			139		Left single quote (European)',
'Alt+Shift+W	140		�thel, latin letter of from Greek OI, like English dipthong OI',
'Alt+Shift+Z	142		Z with inverted circumflex',
'H/Shift+Z		142		Z with inverted circumflex',
'Alt+;			145		Left single curved quote',
'Alt+\'		146		Right single curved quote',
'Alt+Shift+;		147		Left double curved quote',
'Alt+Shift+\'	148		Right double curved quote',
'Alt+8		149		Bullet (typography)',
'Alt+-			150		Dash',
'Alt+Shift+-		151		Em dash',
'Alt+Shift+`	152		Upper tilde',
'U/Shift+`		152		Upper tilde',
'U/T			153		Trademark',
'H/S			154		s with inverted circumflex',
'Alt+]			155		Right single quote (European)',
'Alt+W		156		�thel, latin letter of from Greek OI, like English dipthong OI',
'Alt+Z		158		z with inverted circumflex',
'H/Z			158		z with inverted circumflex',
'D/Shift+Y		159		Y, Diaeresis',
'Alt+ 			160		Non-breaking space',
'Alt+Shift+1	161		Inverted Exclamation Mark',
'Alt+4		162		Cent Sign',
'Alt+Shift+P	163		Pound Sign',
'U/Shift+8		164		Currency Sign',
'Alt+=		165		Yen Sign',
'Alt+Shift+\		166		Broken Bar (',
'Alt+S		167		Section Sign',
'U/Shift+;		168		Diaeresis',
'Alt+Shift+2	169		Copyright Sign',
'U/Shift+C		169		Copyright Sign',
'U/A			170		Feminine Ordinal Indicator',
'Alt+Shift+[		171		Left double quote (European)',
'Alt+Enter		172		Logical not',
'U/-			173		Soft Hyphen (SHY)',
'U/Shift+R		174		Registered Trade Mark Sign',
'U/Shift+-		175		Macron (diacritic)',
'Alt+Shift+8	176		Degree Sign',
'U/8			176		Degree Sign',
'Alt+Shift+=	177		Plus/minus',
'U/2			178		Superscript Two',
'U/3			179		Superscript Three',
'Alt+`		180		Acute Accent (Forward accent)',
'Alt+M		181		Mu (micro)',
'Alt+P		182		Paragraph',
'Alt+.			183		Middle dot (intrapunct)',
'U/.			183		Middle dot (intrapunct)',
'Alt+Shift+,		184		Cedilla (diacritical)',
'U/1			185		Superscript One',
'U/O			186		Masculine Ordinal Indicator',
'Alt+Shift+]		187		Right double quote (European)',
'U/Shift+4		188		One quarter',
'U/Shift+2		189		One half',
'U/Shift+3		190		Three quarters',
'Alt+Shift+/		191		Inverted Question Mark',
'L/Shift+A		192		A, Grave accent',
'R/Shift+A		193		A, Acute accent',
'H/Shift+A		194		A, Circumflex',
'T/Shift+A		195		A, Tilde',
'D/Shift+A		196		A, Diaeresis',
'Alt+Shift+G	197		A, Ring (as in Angstrom)',
'Alt+Shift+Q	198		Old english/Latin letter Ash, originally like English dipthong AI = long I',
'Alt+Shift+C	199		C, Cedilla',
'L/Shift+E		200		E, Grave accent',
'R/Shift+E		201		E, Acute accent',
'H/Shift+E		202		E, Circumflex',
'D/Shift+E		203		E, Diaeresis',
'L/Shift+I		204		I, Grave accent',
'R/Shift+I		205		I, Acute accent',
'H/Shift+I		206		I, Circumflex',
'D/Shift+I		207		I, Diaeresis',
'Alt+Shift+D	208		Old english/Icelandic letter Eth, originally like Greek delta',
'Alt+Shift+N	209		N, tilde',
'L/Shift+O		210		O, Grave accent',
'R/Shift+O		211		O, Acute accent',
'H/Shift+O		212		O, Circumflex',
'T/Shift+O		213		O, Tilde',
'D/Shift+O		214		O, Diaeresis',
'Alt+X		215		Multiplication (cross product)',
'Alt+Shift+R	216		O with slash, Danish vowel, not Null Set',
'L/Shift+U		217		U, Grave accent',
'R/Shift+U		218		U, Acute accent',
'H/Shift+U		219		U, Circumflex',
'D/Shift+U		220		U, Diaeresis',
'Alt+Shift+Y	221		Y, Acute accent',
'R/Shift+Y		221		Y, Acute accent',
'Alt+Shift+B	222		Old/english/Icelandic letter thorn',
'Alt+Shift+S	223		German Sharp S',
'Alt+Shift+A	224		a, Grave accent',
'L/A			224		a, Grave accent',
'Alt+A		225		a, Acute accent',
'R/A			225		a, Acute accent',
'H/A			226		a, Circumflex',
'T/A			227		a, Tilde',
'D/A			228		a, Diaeresis',
'Alt+G		229		a, Ring (as in Angstrom)',
'Alt+Q		230		Old english/Latin letter Ash, originally like English dipthong AI = long I',
'Alt+C		231		c, Cedilla',
'Alt+Shift+E	232		e, Grave accent',
'L/E			232		e, Grave accent',
'Alt+E		233		e, Acute accent',
'R/E			233		e, Acute accent',
'H/E			234		e, Circumflex',
'D/E			235		e, Diaeresis',
'Alt+Shift+I		236		i, Grave accent',
'L/I			236		i, Grave accent',
'Alt+I			237		i, Acute accent',
'R/I			237		i, Acute accent',
'H/I			238		i, Circumflex',
'D/I			239		i, Diaeresis',
'Alt+D		240		Old english/Icelandic letter Eth, originally like Greek delta',
'Alt+N		241		n, tilde',
'T/N			241		n, tilde',
'Alt+Shift+O	242		o, Grave accent',
'L/O			242		o, Grave accent',
'Alt+O		243		o, Acute accent',
'R/O			243		o, Acute accent',
'H/O			244		o, Circumflex',
'T/O			245		o, Tilde',
'D/O			246		o, Diaeresis',
'Alt+/			247		Division Sign',
'Alt+R		248		O with slash, Danish vowel, not Null Set',
'Alt+Shift+U	249		u, Grave accent',
'L/U			249		u, Grave accent',
'Alt+U		250		u, Acute accent',
'R/U			250		u, Acute accent',
'H/U			251		u, Circumflex',
'D/U			252		u, Diaeresis',
'Alt+Y		253		y, Acute accent',
'R/Y			253		y, Acute accent',
'Alt+Shift+B	254		Old/english/Icelandic letter thorn',
'D/Y			255		y, Diaeresis',
]
