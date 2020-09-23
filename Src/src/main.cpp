#include <Arduino.h>
#include <pin.h>

void Taska(void *data)
{
    for (;;)
    {
        digitalWrite(LED, HIGH);
        delay(500);
        digitalWrite(LED, LOW);
        delay(500);
    }
}

void setup()
{
    pinMode(LED, OUTPUT);
    //堆太小会溢出
    xTaskCreate(Taska, "TaskA", 8192, NULL, 2, NULL);
}

void loop()
{
}