int LED_PIN = 2; // A debug LED

// The heating element PWM pins
int HEATER1 = 3;
int HEAT_COOL1 = 4; // high for heating, low for cooling
int HEATER2 = 5;

// Which hand this is
int HAND = 0; // 0 for right, 1 for left

void setup() {
  delay(2);
  // Setup serial
  Serial.begin(115200);

  // Setup LED debug output
  pinMode(LED_PIN,OUTPUT);
  digitalWrite(LED_PIN,LOW);

  // Setup Heating pin outputs
  pinMode(HEATER1,OUTPUT);
  pinMode(HEAT_COOL1,OUTPUT);
  pinMode(HEATER2,OUTPUT);
  digitalWrite(HEATER1,HIGH); // Default to off (same as HEAT_COOL)
  digitalWrite(HEAT_COOL1,HIGH); // Default to heating
}

// Buffer the command
// A command is 4 elements long
int buf[4];
int numElements = 0;


// 5 types of temp
double coldpercent = .60;
double hotpercent = .25;
int isHot = 2;
int currentTempVal = 0;
int starttemp = 0;
//int previousTempType = 3;
// Temperatures turn on and off periodically
unsigned long lastUpdateTime = 0;
// Over time increase the amount of off time
unsigned long offTimeDelay = 0;
unsigned int offTimeIncrease = 10;
/*
void Hot(){
  if(lastUpdateTime + 100 > millis()){
    digitalWrite(HEAT_COOL1,HIGH);
    analogWrite(HEATER1,0);
  } else if(lastUpdateTime + 100 + 300 + offTimeDelay > millis()){
    digitalWrite(HEAT_COOL1,HIGH);
    analogWrite(HEATER1,255);
  } else {
    lastUpdateTime = millis();
    offTimeDelay += offTimeIncrease;
  }
}
void Warm(){
  if(lastUpdateTime + 100 > millis()){
    digitalWrite(HEAT_COOL1,HIGH);
    analogWrite(HEATER1,0);
  } else if(lastUpdateTime + 100 + 700 + offTimeDelay > millis()){
    digitalWrite(HEAT_COOL1,HIGH);
    analogWrite(HEATER1,255);
  } else {
    lastUpdateTime = millis();
    offTimeDelay += offTimeIncrease;
  }
}
void Cool(){
  if(lastUpdateTime + 100 > millis()){
    digitalWrite(HEAT_COOL1,LOW);
    analogWrite(HEATER1,255);
  } else if(lastUpdateTime + 100 + 100 + offTimeDelay > millis()){
    digitalWrite(HEAT_COOL1,LOW);
    analogWrite(HEATER1,0);
  } else {
    lastUpdateTime = millis();
    offTimeDelay += offTimeIncrease / 2;
  }
}
void Cold(){
  if(lastUpdateTime + 100 > millis()){
    digitalWrite(HEAT_COOL1,LOW);
    analogWrite(HEATER1,255);
  } else if(lastUpdateTime + 100 + 50 + offTimeDelay > millis()){
    digitalWrite(HEAT_COOL1,LOW);
    analogWrite(HEATER1,0);
  } else {
    lastUpdateTime = millis();
    offTimeDelay += offTimeIncrease / 2;
  }
}
*/

// Loop forever
void loop() {

  digitalWrite(LED_PIN,LOW);

  // If we have a whole packet (4 chars)
  if(numElements == 4){
    
    if(buf[0] == 254){
      currentTempVal = buf[2];
      isHot = buf[3];
      starttemp = buf[2];
      /*Serial.println("BUF");
      Serial.println(buf[0]);
      Serial.println(buf[1]);
      Serial.println(buf[2]);
      Serial.println(buf[3]);
      Serial.println("===");*/
      lastUpdateTime = 0;
      offTimeDelay = 0;
    }
    numElements = 0;

  // else we are collecting chars for the new packet
  } else {
    // Check if new serial byte
    if(Serial.available()){
      // Read byte
      int c = Serial.read();
      if(c >= 0){
        buf[numElements] = c;
        numElements++;

      // If byte was an error
      } else {
        digitalWrite(LED_PIN,HIGH);
        delay(100);
      }
    }
  }

  //Update Temp

  //if it is cooling
  //Serial.print("isHot: ");
  ///Serial.println(isHot);

    if(isHot == 0)  
    {
      if(lastUpdateTime + 100 > millis()){
      digitalWrite(HEAT_COOL1,LOW);
      analogWrite(HEATER1,(currentTempVal * coldpercent));
    } else if(lastUpdateTime + 100 + offTimeDelay > millis()){
      digitalWrite(HEAT_COOL1,LOW);
      analogWrite(HEATER1,0);
    } else {
      lastUpdateTime = millis();
      offTimeDelay += offTimeIncrease / 2;
    }
      
      /*
      digitalWrite(HEAT_COOL1,LOW);
      //analogWrite(HEATER1,(60));
      if (FIRST_TIME == 1) {
        analogWrite(HEATER1,(currentTempVal * coldpercent));
        delay(2000);
        analogWrite(HEATER1,50);
        delay(1000);
      }
      else 
        analogWrite(HEATER1,(currentTempVal * coldpercent));
      currentTempVal = 60;
      FIRST_TIME = 0;
     // if(currentTempVal > (starttemp * coldpercent))
      //{
      //  currentTempVal = (currentTempVal * 0.90);
     // }*/
     //Serial.print("Cold: ");
    // Serial.println(currentTempVal * coldpercent);
     
    }
    else if(isHot == 1)//if it is heating
    {
       if(lastUpdateTime + 100 > millis())
       {
          digitalWrite(HEAT_COOL1,HIGH);
          analogWrite(HEATER1,(255-(currentTempVal * hotpercent)));
       } 
       else if(lastUpdateTime + 100 + offTimeDelay > millis())
          {
            digitalWrite(HEAT_COOL1,HIGH);
            analogWrite(HEATER1,255);
          }
          else 
            {
              lastUpdateTime = millis();
              offTimeDelay += offTimeIncrease;
            }
      /*
      digitalWrite(HEAT_COOL1,HIGH);
      //analogWrite(HEATER1,(255 - 60));
      analogWrite(HEATER1,(255-(currentTempVal * hotpercent)));
      //if(currentTempVal > (starttemp * hotpercent))
      //{
       // currentTempVal = (currentTempVal * 0.70);
      //}
     Serial.print("Hot: ");
     Serial.println(255-(currentTempVal * hotpercent));
     */
    }
    else {
      digitalWrite(HEAT_COOL1,LOW);
      analogWrite(HEATER1,0);
    }
  }
 
  /*

  // Update temp
  if(currentTempVal > 200){
    // IF warm or hot, don't reset time delay
    if( previousTempType != 1 && previousTempType != 2 ){
      offTimeDelay = 0;
    }
    previousTempType = 1;
    Hot();
    
  } else if(currentTempVal > 159) {
    // IF warm or hot, don't reset time delay
    if(previousTempType != 1 && previousTempType != 2){
      offTimeDelay = 0;
    }
    previousTempType = 2;
    Warm();
    
  } else if(currentTempVal > 95) {
    if(previousTempType != 3){
      offTimeDelay = 0;
    }
    previousTempType = 3;
    
    // Room temp!!!
    analogWrite(HEATER1,255);
    digitalWrite(HEAT_COOL1,HIGH);
    
  } else if(currentTempVal > 32){
    if(previousTempType != 4){
      offTimeDelay = 0;
    }
    previousTempType = 4;
    Cool();
    
  } else {
    if(previousTempType != 5){
      offTimeDelay = 0;
    }
    previousTempType = 5;
    Cold();
    
  }
  */
