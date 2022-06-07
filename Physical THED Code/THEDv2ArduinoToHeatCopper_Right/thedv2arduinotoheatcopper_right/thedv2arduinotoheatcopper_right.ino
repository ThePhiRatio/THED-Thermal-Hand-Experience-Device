int LED_PIN = 0; // A debug LED

// The heating element PWM pins
int HEATER1 = 3;
int HEAT_COOL1 = 4; // high for heating, low for cooling
int HEATER2 = 2;

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





//NEED TO CHANGE THINGS BELOW FOR NEW VALUES




// Buffer the command
// A command is 4 elements long
int buf[6];
int numElements = 0;

//variables
double coldpercent = .37;
double hotpercent = .08;
int isHot = 2;
int currentTempVal = 0;
int starttemp = 0;

// Temperatures turn on and off periodically
unsigned long lastUpdateTime = 0;
// Over time increase the amount of off time
unsigned long offTimeDelay = 0;
unsigned int offTimeIncrease_cold = 8;  //originally 10
unsigned int offTimeIncrease_hot = 40;


void loop() {

digitalWrite(LED_PIN,LOW);

  // If we have a whole packet (4 chars)
  if(numElements == 6){
    
    if(buf[0] == 254){
      currentTempVal = buf[3];
      isHot = buf[5];
      starttemp = buf[3];
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
    if(isHot == 0)  
    {
      if(lastUpdateTime + 100 > millis()){
        digitalWrite(HEAT_COOL1,LOW); //4
        digitalWrite(HEATER2,HIGH); // 2
        analogWrite(HEATER1,(currentTempVal * coldpercent));
    } else if(lastUpdateTime + 100 + offTimeDelay > millis()){
        digitalWrite(HEAT_COOL1,LOW); //4
        digitalWrite(HEATER2,HIGH); // 2
        analogWrite(HEATER1,0);
    } else {
      lastUpdateTime = millis();
      offTimeDelay += offTimeIncrease_cold;
    }
    }

    //if it is heating
    else if(isHot == 1)
    {
       if(lastUpdateTime + 100 > millis())
       {
          digitalWrite(HEAT_COOL1,HIGH); //4
          digitalWrite(HEATER2,LOW); // 2
          analogWrite(HEATER1,(currentTempVal * hotpercent));
       } 
       else if(lastUpdateTime + 100 + offTimeDelay > millis())
          {
            digitalWrite(HEAT_COOL1,HIGH); //4
            digitalWrite(HEATER2,LOW); // 2
            analogWrite(HEATER1,0);
          }
          else 
            {
              lastUpdateTime = millis();
              offTimeDelay += offTimeIncrease_hot;                 //offTimeDelay += offTimeIncrease;
            }
    }

     else {
      digitalWrite(HEAT_COOL1,LOW);
      analogWrite(HEATER1,0);
    }

         
}


    
