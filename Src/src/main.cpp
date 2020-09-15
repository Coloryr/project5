#include <Arduino.h>
#include <pin.h>

void Taska(void *data)
{
    portTickType xLastWakeTime;
    xLastWakeTime = xTaskGetTickCount();
    for (;;)
    {
        digitalWrite(LED, HIGH);
        vTaskDelayUntil(&xLastWakeTime, (100 / portTICK_RATE_MS));
        digitalWrite(LED, LOW);
        vTaskDelayUntil(&xLastWakeTime, (100 / portTICK_RATE_MS));
    }
}

void setup()
{
    xTaskCreate(Taska, "TaskA", 80, NULL, 2, NULL);
    vTaskStartScheduler();
}

void loop()
{
    for (;;)
    {
    }
}