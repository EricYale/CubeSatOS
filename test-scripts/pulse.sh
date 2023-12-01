#!/bin/bash

raspi-gpio set 26 op
raspi-gpio set 19 op

raspi-gpio set 13 op
raspi-gpio set 11 op

raspi-gpio set 5 op
raspi-gpio set 0 op

raspi-gpio set 26 dh
raspi-gpio set 19 dh

raspi-gpio set 13 dh
raspi-gpio set 11 dh

raspi-gpio set 5 dh
raspi-gpio set 0 dh

while [ 1 ]
do
	raspi-gpio set 26 dl
	sleep 0.5
	raspi-gpio set 26 dh
	raspi-gpio set 19 dl
	sleep 0.5
	raspi-gpio set 19 dh
	raspi-gpio set 13 dl
	sleep 0.5
	raspi-gpio set 13 dh
	raspi-gpio set 11 dl
	sleep 0.5
	raspi-gpio set 11 dh
	raspi-gpio set 5 dl
	sleep 0.5
	raspi-gpio set 5 dh
	raspi-gpio set 0 dl
	sleep 0.5
	raspi-gpio set 0 dh
done
