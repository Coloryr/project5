#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "sdkconfig.h"
#include "TaskUart.h"
#include "TaskShow.h"
#include "HC138.h"
#include "HC595.h"
#include "Uart.h"
#include "DataSave.h"

TaskHandle_t Start_Task_Handler;

void TaskInit(void *data)
{
    printf("正在初始化\n");
    HC138Init();
    HC595Init();
    UartInit();
    DataInit();

    xTaskCreate(TaskShow, "TaskShow", 2048, NULL, 2, NULL);
    xTaskCreate(TaskUart, "TaskUart", 2048, NULL, 2, NULL);

    vTaskDelete(Start_Task_Handler);
}

void app_main()
{
    vTaskDelay(500 / portTICK_PERIOD_MS);
    xTaskCreate(TaskInit, "TaskInit", 1024, NULL, 2, &Start_Task_Handler);
}