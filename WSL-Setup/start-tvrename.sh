#!/bin/bash

testpath="/home/andy/tvrename/assets"
process="jekyll serve"

status=`ps aux|grep "${process}" | grep -v grep` 

if [ ! -z "${status}" ]
then
	echo "Jekyll already running..."
	exit
fi
#
if [ ! -d "${testpath}" ]
then
	echo Mapping Path...
	sudo mount --bind /mnt/c/Users/Andy/tvrename /home/andy/tvrename
else
	echo "Drive already Mapped..."
fi
cd /home/andy/tvrename
bundle exec jekyll serve
