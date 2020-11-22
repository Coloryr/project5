#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "driver/gpio.h"
#include "sdkconfig.h"
#include "DataSave.h"

uint8_t Height;
uint8_t Width;
uint8_t CanRun;

uint8_t **REDData;
uint8_t **BULData;

void DataInit()
{
    Height = 0;
    Width = 0;
    CanRun = 1;

    REDData = NULL;
    BULData = NULL;
}

void SetRun(uint8_t *data, uint8_t start)
{
    uint16_t i;
    CanRun = 0;
    vTaskDelay(50 / portTICK_PERIOD_MS);

    if (REDData != NULL)
    {
        for (int i = 0; i < Width; i++)
            free(REDData[i]);
        free(REDData);
    }
    if (BULData != NULL)
    {
        for (int i = 0; i < Width; i++)
            free(BULData[i]);
        free(BULData);
    }

    Height = data[start];
    Width = data[start + 1] / 8;

    REDData = (uint8_t **)malloc(Height * sizeof(int *));
    BULData = (uint8_t **)malloc(Height * sizeof(int *));
    for (uint8_t i = 0; i < Height; i++)
    {
        REDData[i] = (uint8_t *)malloc(Width * sizeof(int));
        BULData[i] = (uint8_t *)malloc(Width * sizeof(int));
    }

    CanRun = 1;
}

void SetShow(uint8_t *data, uint8_t start)
{
    uint8_t count = start;
    uint8_t next = start + Height * Width;
    for (uint8_t i = 0; i < Height; i++)
    {
        for (uint8_t j = 0; j < Width; j++)
        {
            REDData[i][j] = data[count++];
            BULData[i][j] = data[next++];
        }
    }
}
