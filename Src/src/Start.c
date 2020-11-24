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

void app_main()
{
    vTaskDelay(500 / portTICK_PERIOD_MS);
    printf("正在初始化\n");
    HC138Init();
    HC595Init();
    UartInit();
    DataInit();
    xTaskCreatePinnedToCore(TaskShow, "TaskShow", 2048, NULL, 2, NULL, 1);
    xTaskCreatePinnedToCore(TaskUart, "TaskUart", 2048, NULL, 2, NULL, 0);
}
