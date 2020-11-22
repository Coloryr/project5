#ifndef HC138_h
#define HC138_h

#include <stdio.h>

//74HC138
#define A0 2
#define A1 4
#define A2 13
#define A3 16
#define AEN 15

extern uint8_t local;

void HC138Init();
void SetPin();
void SetEnable(uint8_t enable);
void AddPos();
void ReSetPos();
void SetPos(uint8_t pos);

#endif