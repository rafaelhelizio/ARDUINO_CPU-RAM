/*
 * Author: Rafael Helizio Serrate Zago
 * License: GNU General Public License v3.0
 * Ver.:0.1
 */

#include <Arduino_JSON.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1331.h>

#define sclk 13
#define mosi 11
#define cs   10
#define rst  9
#define dc   8

#define RAM_MED 30
#define RAM_MAX 80

#define CPU_MED 40
#define CPU_MAX 90

// Color definitions
#define BLACK           0x0000
#define RED             0xF800
#define GREEN           0x07E0
#define YELLOW          0xFFE0  

char c[100];


Adafruit_SSD1331 display = Adafruit_SSD1331(cs, dc, rst);

void setup() {
  Serial.begin(9600);
  display.begin();
  display.fillScreen(BLACK);
  
}

void loop() 
{
  if(Serial.available())        
  {
     String input = Serial.readString();  
     input.toCharArray(c, 100);

      JSONVar myObject = JSON.parse(c);
    
      // JSON.typeof(jsonVar) can be used to get the type of the var
      if (JSON.typeof(myObject) == "undefined") {
        Serial.println("Parsing input failed!");
        display.print("Parsing input failed!");
        return;
      }
      int CPU = (int)myObject["C"];
      int RAM = (int)myObject["R"];
  
      if(CPU >= CPU_MED){
        if(CPU>=CPU_MAX){
          display.fillRect(0,0,96,33,RED);
         }else{
          display.fillRect(0,0,96,33,YELLOW);
          }
      }else{
       display.fillRect(0,0,96,33,GREEN);
      }
  
      if(RAM >= RAM_MED){
        if(RAM>=RAM_MAX){
          display.fillRect(0,34,96,64,RED);
         }else{
          display.fillRect(0,34,96,64,YELLOW);
          }
        
      }else{
       display.fillRect(0,34,96,64,GREEN);
      }
  
      String CPU_STR = String(CPU);   
      String RAM_STR = String(RAM);   
  
      CPU_STR+="%";
      RAM_STR+="%";
      
      display.setTextColor(BLACK);
      
      display.setTextSize(1);
      display.setCursor(3,4);
      display.print("C"); 
      display.setCursor(3,14);
      display.print("P"); 
      display.setCursor(3,24);
      display.print("U");
      display.setCursor(3,35);
      display.print("R"); 
      display.setCursor(3,45);
      display.print("A"); 
      display.setCursor(3,55);
      display.print("M");
      
      display.setTextSize(3); 
       
      display.setCursor(20,5);
      display.print(CPU_STR); 
  
      display.setCursor(20,40);
      display.print(RAM_STR); 
      delay(1000);
  }
}
