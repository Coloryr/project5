#include <Arduino.h>
#include <pin.h>
#include <esp_spi_flash.h>

//任务A
void Taska(void *data)
{
    //循环闪烁
    for (;;)
    {
        //写IO口为高
        digitalWrite(LED, HIGH);
        //延迟
        delay(500);
        //写IO口为低
        digitalWrite(LED, LOW);
        //延迟
        delay(500);
    }
}

void setup()
{
    //设置引脚模式
    pinMode(LED, OUTPUT);
    //创建任务
    xTaskCreate(
        Taska,   //任务
        "TaskA", //任务名字
        8192,    //堆大小，太小会溢出
        NULL,    //参数
        2,       //优先级
        NULL     //控制指针
    );
}

void loop()
{
}