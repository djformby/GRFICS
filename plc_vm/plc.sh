#!/bin/bash
apt-get -y update
apt-get -y upgrade
apt-get -y install locate dnsutils lsof
apt-get -y install vim-gtk subversion build-essential pkg-config bison flex autoconf automake libtool make nodejs git
svn export https://github.com/Valiant-Mouse/GRFICS/trunk/plc_vm
pwd | grep '/home/' &> /dev/null
if [ $? == 1 ]; then
  ls |grep 'plc_vm' &> /dev/null
  if [ $? == 0 ]; then
    mv plc_vm ~
  fi
fi
cd ~/plc_vm/OpenPLC_v2-master
sudo bash ./build-noninteractive-MODBUS-only.sh
sudo nodejs server.js
