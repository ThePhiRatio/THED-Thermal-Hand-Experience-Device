int LED_PIN = 2; // A debug LED

// The heating element PWM pins
int HEATER1 = 3;
int HEAT_COOL1 = 4; // high for heating, low for cooling
int HEATER2 = 5;

// Which hand this is
int HAND = 0; // 0 for right, 1 for left

void setup() {
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
int currentTempVal = 127;
int previousTempType = 3;
// Temperatures turn on and off periodically
unsigned long lastUpdateTime = 0;
// Over time increase the amount of off time
unsigned long offTimeDelay = 0;
unsigned int offTimeIncrease = 10;

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


// Loop forever
void loop() {

  digitalWrite(LED_PIN,LOW);
  

  // If we have a whole packet (4 chars)
  if(numElements == 4){

    if(buf[0] == 254){
      currentTempVal = buf[2];
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
        delay(1000);
      }
    }
  }

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
}
