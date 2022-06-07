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



// Loop forever
void loop() {

  digitalWrite(LED_PIN,LOW);

  /*// cooling
  digitalWrite(HEAT_COOL1,LOW); //4
  digitalWrite(HEATER2,HIGH); // 2
  analogWrite(HEATER1,0); // 3*/

  // heating
  digitalWrite(HEAT_COOL1,HIGH); //4
  digitalWrite(HEATER2,LOW); // 2
  analogWrite(HEATER1,50); // 3 
}
