            #!/bin/bash
            apt-get -y update
            apt-get -y upgrade
            apt-get -y install locate dnsutils lsof
            apt-get -y install vim-gtk subversion build-essential pkg-config bison flex autoconf automake libtool make nodejs git
            svn export https://github.com/Valiant-Mouse/GRFICS/trunk/plc_vm
            mv plc_vm /home/user
            cd /home/user/plc_vm/OpenPLC_v2-master
            sudo bash ./build-noninteractive-MODBUS-only.sh
            sudo nodejs server.js
