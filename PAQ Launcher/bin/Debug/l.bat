@echo off
cd %cd%\bin
java -Xms%1 -Xmx%2 -XX:MaxPermSize=%3 -Djava.library.path=natives/ -cp  "minecraft.jar;lwjgl.jar;lwjgl_util.jar" net.minecraft.client.Minecraft %4 %5
pause