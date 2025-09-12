#!/bin/bash

# Esperar a que MySQL esté disponible en el puerto 3307
echo "Esperando a MySQL en mysql-db:3307..."
while ! nc -z mysql-db 3307; do
  sleep 1
  echo "Esperando a MySQL..."
done

echo "MySQL está disponible en puerto 3307, iniciando aplicación..."
exec dotnet WebApp.dll