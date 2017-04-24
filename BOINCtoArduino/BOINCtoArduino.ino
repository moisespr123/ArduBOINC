#include <LiquidCrystal.h>

LiquidCrystal lcd(12, 11, 5, 4, 3, 2);
String wuname = "";
String project = "";
String percent = "";
String ETA = "";
void setup() {
  lcd.begin(16, 2);
  lcd.setCursor(0, 0);
  lcd.print("BOINC Tasks To");
  lcd.setCursor(0, 1);
  lcd.print("Arduino");
  Serial.begin(38400);
}

void loop() {
  int currentinfo = 0;
  wuname = "";
  project = "";
  percent = "";
  ETA = "";
  while (Serial.available() == 0)
  {
    currentinfo = 0;
    wuname = "";
    project = "";
    percent = "";
    ETA = "";
  }
  while (Serial.available() > 0) {
    char character = Serial.read();
    if (character == '|')
      currentinfo++;
    if (currentinfo == 0 && character != '|')
      wuname += character;
    else if (currentinfo == 1 && character != '|')
      project += character;
    else if (currentinfo == 2 && character != '|')
      percent += character;
    else if (currentinfo == 3 && character != '|')
      ETA += character;
  }
  if (project.length() < 16)
  {
    for (int i = project.length(); i < 15; i++)
      project += " ";
  }
  if (percent.length() < 16)
  {
    for (int i = percent.length(); i < 15; i++)
      percent += " ";
  }
  if (ETA.length() < 16)
  {
    for (int i = ETA.length(); i < 15; i++)
      ETA += " ";
  }
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(wuname);
  lcd.setCursor(0, 1);
  lcd.print(project);
  delay(3000);
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(wuname);
  lcd.setCursor(0, 1);
  lcd.print(percent);
  delay(3000);
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(wuname);
  lcd.setCursor(0, 1);
  lcd.print(ETA);
  delay(3000);
  Serial.println("OK");
}

