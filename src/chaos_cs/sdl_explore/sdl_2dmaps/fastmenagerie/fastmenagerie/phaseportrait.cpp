#include "phaseportrait.h"
#include "float_cast.h"
#include "whichmap.h"
#include "palette.h"

//note that these functions don't use the a and b from g_settings.
//a "seed point" is an initial point. In some maps, choice of (x0,y0) changes behavior.
void DrawPhasePortrait( SDL_Surface* pSurface, double c1, double c2, int width , int px0 ) 
{
	double sx0= g_settings->seedx0, sx1=g_settings->seedx1, sy0= g_settings->seedy0, sy1=g_settings->seedy1;
	int nXpoints=g_settings->seedsPerAxis; int nYpoints=g_settings->seedsPerAxis;
	int height=width;
	double sxinc = (nXpoints==1) ? 1e6 : (sx1-sx0)/(nXpoints-1);
	double syinc = (nYpoints==1) ? 1e6 : (sy1-sy0)/(nYpoints-1);

	double x_,y_,x,y;
	double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;

	for (double sx=sx0; sx<=sx1; sx+=sxinc)
            {
                for (double sy=sy0; sy<=sy1; sy+=syinc)
                {
                    x = sx; y=sy;

					for (int ii=0; ii<(g_settings->settlingTime); ii++)
                    {
						MAPEXPRESSION;
                        x=x_; y=y_;
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;
                    }
					for (int ii=0; ii<(g_settings->drawingTime); ii++)
                    {
						MAPEXPRESSION;
                        x=x_; y=y_;
						if (ISTOOBIG(x)||ISTOOBIG(y)) break;

// DRAWPERIODIC means to draw many copies of the figure. set in whichmap.h
#if DRAWPERIODIC
						for (double rx=x-6.2831853*2; rx<x+6.2831853*2;rx+=6.2831853) {
						for (double ry=y-6.2831853*2; ry<y+6.2831853*2;ry+=6.2831853) {
#else
						double rx=x, ry=y;
#endif
                        int px = lrint(width * ((rx - X0) / (X1 - X0)));
                        int py = lrint(height - height * ((ry - Y0) / (Y1 - Y0)));
                        if (py >= 0 && py < height && px>=0 && px<width)
						{

  Uint32 col = 0 ; Uint32 colR;  
  char* pPosition = ( char* ) pSurface->pixels ; //determine position
  pPosition += ( pSurface->pitch * py ); //offset by y
  pPosition += ( pSurface->format->BytesPerPixel * (px+px0) ); //offset by x
  memcpy ( &col , pPosition , pSurface->format->BytesPerPixel ) ; //copy pixel data

  SDL_Color color ;
  SDL_GetRGB ( col , pSurface->format , &color.r , &color.g , &color.b ) ;
  colR = color.r;
							
						Uint32 newcolor = (colR)-((colR)>>2); //add shade to that pixel

  Uint32 newcol = SDL_MapRGB ( pSurface->format , newcolor , newcolor , newcolor ) ;
  memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;
  #if DRAWPERIODIC
						}}
#endif
				}
            }
        }
    }
}

//double largestSeenBasins; It didn't look very good when we used greatest-value to set colors.

BOOL gParamDrawBasinsWithBlueAlso=FALSE;
//estimate "basins of attraction". Pixel is x0,y0, colored by final value after n iterations.
void DrawBasins( SDL_Surface* pSurface, double c1, double c2, int width, int px0) 
{
double fx,fy, x_,y_,x,y; char* pPosition; Uint32 r,g,b, newcol; double val;
int height=width;
double X0=g_settings->x0, X1=g_settings->x1, Y0=g_settings->y0, Y1=g_settings->y1;
double dx = (X1 - X0) / width, dy = (Y1 - Y0) / height;
fx = X0; fy = Y1; //y counts downwards
for (int py=0; py<height; py+=1)
{
	fx=X0;
	for (int px = 0; px < width; px+=1)
	{
		x=fx; y=fy;
		for (int i=0; i<g_settings->basinsTime; i++)
		{
			MAPEXPRESSION;
			x=x_; y=y_;
			if (ISTOOBIG(x)||ISTOOBIG(y)) break;
		}

		if (g_settings->drawingMode==DrawModeBasinsDistance)
		{
			val = sqrt( (x-fx)*(x-fx)+(y-fx)*(y-fx));
		}
		else if (g_settings->drawingMode==DrawModeBasinsX)
		{
			val = sqrt(fabs(x));
		}
		else if (g_settings->drawingMode==DrawModeBasinsDifference)
		{
			double prevx = x; MAPEXPRESSION;
			val = fabs(x_-prevx);
		}
		if (ISTOOBIG(x)||ISTOOBIG(y)) { 
			// dark blue to distinguish from black.
			newcol = SDL_MapRGB( pSurface->format , 0 , 0, 35 ) ; 
		}
		else
		{
			
			if (!gParamDrawBasinsWithBlueAlso) {
				val = val / g_settings->basinsMaxColor;
				if (val>=1.0)
					newcol = SDL_MapRGB( pSurface->format , 220 , 220, 255 );
				else {
					int v = (int)(val*255);
					newcol = SDL_MapRGB( pSurface->format , v,v,v );
				}
			}else {
				//val += 0.5; if (val>1) val-=1;
				//newcol = HSL2RGB(pSurface, val, 0.5, 0.5);
				val = sqrt(val) / sqrt(g_settings->basinsMaxColor);
				if (val>=1.0) val=1.0; if (val<0.0) val=0.0;
				val=val*2-1;
				if (val<=0)
					b=255, r=g= (Uint32) ((1+val)*255.0);
				else
					r=g=b= (Uint32) ((1-val)*255.0);
				newcol = SDL_MapRGB( pSurface->format , r,g,b );
			}
		}
		
		pPosition = ( char* ) pSurface->pixels ; //determine position
		pPosition += ( pSurface->pitch * py ); //offset by y
		pPosition += ( pSurface->format->BytesPerPixel * (px+px0) ); //offset by x
		memcpy ( pPosition , &newcol , pSurface->format->BytesPerPixel ) ;

		fx += dx;
	}
fy -= dy;
}
}



void DrawFigure( SDL_Surface* pSurface, double c1, double c2, int width, int px0 ) 
{
	switch (g_settings->drawingMode)
	{
		case DrawModePhase:  DrawPhasePortrait(pSurface, c1, c2, width, px0); break;
		case DrawModeBasinsDistance:  
		case DrawModeBasinsDifference:  
		case DrawModeBasinsX:  
			DrawBasins(pSurface, c1, c2, width, px0); break;
		default: {massert(0, "Unknown drawing mode."); }
	}
}

void renderLargeFigure( SDL_Surface* pSurface, int width, const char*filename ) 
{
	char filenameext[256];
	snprintf(filenameext, sizeof(filenameext), "%s.bmp", filename);
//create a new surface.
	SDL_Surface* pRenderSurface = SDL_CreateRGBSurface( SDL_SWSURFACE, width, width, pSurface->format->BitsPerPixel, pSurface->format->Rmask, pSurface->format->Gmask, pSurface->format->Bmask, 0 );
	SDL_FillRect ( pRenderSurface , NULL , g_white );
	//increase the amount of drawing.
	int oldDrawingTime = g_settings->drawingTime;
	g_settings->drawingTime *= 32;
	DrawFigure(pRenderSurface, g_settings->a, g_settings->b, width, 0);
	g_settings->drawingTime = oldDrawingTime;
	SDL_SaveBMP(pRenderSurface, filenameext);
	SDL_FreeSurface(pRenderSurface);
}
