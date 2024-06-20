#!/bin/bash
echo 127.0.0.1 $(hostname) >> /etc/hosts
echo 52.247.160.149 git.cybbh.space >> /etc/hosts
sed -i's/PasswordAuthentication no/PasswordAuthentication yes/' /etc/ssh/sshd_config
export DEBIAN_FRONTEND=noninteractive
apt-get update
apt-get -y upgrade
apt-get -y install locate dnsutils lsof freerdp
apt-get -y install vim-gtk subversion wine build-essential pkg-config bison flex autoconf automake libtool make nodejs git
svn export https://github.com/djformby/GRFICS/trunk/simulation_vm
mv simulation_vm /home/user
cd /home/user/
unzip simulation_vm/HMI_Simulation_Ubuntu1604_15_x86_64.zip
useradd $user -m -U -s /bin/bash
usermod -aG sudo $user
echo "root:$password" | chpasswd
echo "$user:$password" | chpasswd
reboot
