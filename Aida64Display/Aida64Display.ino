/*  draw text's APP
    drawChar(INT8U ascii,INT16U poX, INT16U poY,INT16U size, INT16U fgcolor);
    drawString(char *string,INT16U poX, INT16U poY,INT16U size,INT16U fgcolor);
*/

#include <stdint.h>
#include <TFTv2.h>
#include <SPI.h>

boolean CommandStringReceived = false;  //Global flag used to signal a complete command has been received.
String CommandString;  //Global variable containing the command.  
int currentLine = 0;
int currentColor = BLUE;

#define FONTSIZE 2
#define STARTLINE 1

void setup()
{
  Serial.begin(115200);
  TFT_BL_ON;      // turn on the background light
  Tft.TFTinit();  // init TFT library
  Tft.fillScreen(0, 240, 0, 320, BLACK);
  Serial.println("Boot completed, entering run mode.");
}

void loop()
{
  if(CommandStringReceived)
  {
    if(CommandString.indexOf("BEGIN") != -1)
    {
      currentLine = STARTLINE;
    }
    else
    {
      char charBuf[40];
      Tft.fillRectangle(0, currentLine, 240, 25, BLACK);
      
      CommandString.toCharArray(charBuf, 40);
      for(int i = 0;i < 40;i++)
      {
        if(charBuf[i] == '!' || charBuf[i] == '\n')
          charBuf[i] = ' ';
      }
      
      Tft.drawString(charBuf, 0, currentLine, FONTSIZE, currentColor);
      currentLine = currentLine + 25;
    }
    
     CommandStringReceived = false;
     CommandString = "";
  }
}

void serialEvent()
{
   while (Serial.available()) 
   {
     // get the new byte:
     char inChar = (char)Serial.read(); 
    // add it to the inputString:
     CommandString += inChar;
     // if the incoming character is a newline, set a flag
     // so the main loop can do something about it:
     if (inChar == '\n' || inChar == '\r') 
     {
       CommandStringReceived = true;
     } 
   }
}
