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
    Height = 16;
    Width = 4;
    CanRun = 1;

    REDData = NULL;
    BULData = NULL;

    InitShow();
}

void FreeShow()
{
    if (REDData != NULL)
    {
        for (uint8_t i = 0; i < Width; i++)
            free(REDData[i]);
        free(REDData);
    }
    if (BULData != NULL)
    {
        for (uint8_t i = 0; i < Width; i++)
            free(BULData[i]);
        free(BULData);
    }
}

void InitShow()
{
    REDData = (uint8_t **)malloc(Height * sizeof(int *));
    BULData = (uint8_t **)malloc(Height * sizeof(int *));
    for (uint8_t i = 0; i < Height; i++)
    {
        REDData[i] = (uint8_t *)malloc(Width * sizeof(int));
        BULData[i] = (uint8_t *)malloc(Width * sizeof(int));
    }
}

void SetRun(uint8_t *data, uint8_t start)
{
    CanRun = 0;
    vTaskDelay(50 / portTICK_PERIOD_MS);

    FreeShow();

    Height = data[start];
    Width = data[start + 1] / 8;

    InitShow();

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
