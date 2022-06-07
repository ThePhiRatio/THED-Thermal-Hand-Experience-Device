int LED_PIN = 2; // A debug LED




void setup() {
  // Setup serial
  Serial.begin(115200);

  // Setup LED debug output
  pinMode(LED_PIN,OUTPUT);
  digitalWrite(LED_PIN,LOW);
}

// Buffer the command
// A command is 4 elements long
int buf[6];
int numElements = 0;

// Loop forever
void loop() {

  // Debug led is set low every loop
  digitalWrite(LED_PIN,LOW);

  // If we have a whole packet (4 chars)
  if(numElements == 6){

    // Let user know packet was sent
    digitalWrite(LED_PIN,HIGH);

    // Make sure we have the start character
    if(buf[0] == 254){
      Serial.print(buf[0]);
      Serial.print(buf[1]);
      Serial.print(buf[2]);
      Serial.print(buf[3]);
      Serial.print(buf[4]);
      Serial.print(buf[5]);
      Serial.flush();
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
}
