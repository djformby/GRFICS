#!/bin/bash
echo Building OpenPLC environment:

echo [MATIEC COMPILER]
cd matiec_src
autoreconf -i
./configure
make

echo [LADDER]
cd ..
cp ./matiec_src/iec2c ./
./iec2c ./st_files/blank_program.st
mv -f POUS.c POUS.h LOCATED_VARIABLES.h VARIABLES.csv Config0.c Config0.h Res0.c ./core/

echo [ST OPTIMIZER]
cd st_optimizer_src
g++ st_optimizer.cpp -o st_optimizer
cd ..
cp ./st_optimizer_src/st_optimizer ./

echo [GLUE GENERATOR]
cd glue_generator_src
g++ glue_generator.cpp -o glue_generator
cd ..
cp ./glue_generator_src/glue_generator ./core/glue_generator

clear

echo Skipping DNP3 installation
mv ./core/dnp3.cpp ./core/dnp3.disabled 2> /dev/null
mv ./core/dnp3_dummy.disabled ./core/dnp3_dummy.cpp 2> /dev/null
cp -f ./core/core_builders/dnp3_disabled/*.* ./core/core_builders/

cd core
rm -f ./hardware_layer.cpp
rm -f ../build_core.sh

cp ./hardware_layers/modbus_master.cpp ./hardware_layer.cpp
cp ./core_builders/build_modbus.sh ../build_core.sh
echo [LIBMODBUS]
cd ..
cd libmodbus-3.0.4
./autogen.sh
./configure
sudo make install
sudo ldconfig
echo [OPENPLC]
cd ..
./build_core.sh
exit

