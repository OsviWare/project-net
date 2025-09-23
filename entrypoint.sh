#!/bin/bash

# Esperar a que MySQL esté disponible en el puerto 3306 (interno)
echo "Esperando a MySQL en mysql-db:3306..."
while ! nc -z mysql-db 3306; do
  sleep 1
  echo "Esperando a MySQL..."
done

echo "MySQL está disponible en puerto 3306, iniciando aplicación..."
exec dotnet WebApp.dll