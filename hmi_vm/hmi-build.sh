#!/bin/bash
useradd user -m -U -s /bin/bash
usermod -aG sudo user
echo "root:password" | chpasswd
echo "user:password" | chpasswd
apt-get update
apt-get -y install vim-gtk locate lsof subversion wine python-wxgtk3.0 pyro python-numpy python-nevow python-matplotlib python-lxml python-zeroconf
svn export https://github.com/thiagoralves/OpenPLC_Files/trunk/Software/PLCopen%20Editor%20v1.3%20-%20Linux.zip
unzip 'PLCopen Editor 1.3 - Linux.zip'
svn export https://github.com/djformby/GRFICS/trunk/plc_vm
svn export https://github.com/djformby/GRFICS/trunk/hmi_vm
cd hmi_vm/HMI
wine AdvancedHMI.exe
