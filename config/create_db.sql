-- Crear base de datos
CREATE DATABASE IF NOT EXISTS appdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE appdb;

-- Tabla roles
CREATE TABLE IF NOT EXISTS roles (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50) UNIQUE NOT NULL,
    descripcion TEXT
);

-- Datos iniciales de roles
INSERT INTO roles (nombre, descripcion) VALUES 
('administrador', 'Administrador completo del sistema'),
('usuario_sistema', 'Usuario que puede vender y comprar'),
('externo', 'Usuario que solo puede comprar');

-- Tabla usuarios
CREATE TABLE IF NOT EXISTS usuarios (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    telefono VARCHAR(20),
    direccion TEXT,
    fecha_registro DATETIME DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN DEFAULT TRUE,
    rol_id INT NOT NULL,
    FOREIGN KEY (rol_id) REFERENCES roles(id)
);

-- Tabla categorias
CREATE TABLE IF NOT EXISTS categorias (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100) UNIQUE NOT NULL,
    descripcion TEXT
);

-- Datos iniciales de categorias
INSERT INTO categorias (nombre, descripcion) VALUES 
('Ropa y Accesorios', 'Prendas de vestir y complementos'),
('Tecnología', 'Dispositivos electrónicos y tecnología'),
('Hogar y Muebles', 'Artículos para el hogar y muebles'),
('Deportes', 'Equipamiento y ropa deportiva'),
('Libros', 'Libros de todo tipo');

-- Tabla productos
CREATE TABLE IF NOT EXISTS productos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(200) NOT NULL,
    descripcion TEXT,
    precio DECIMAL(10,2) NOT NULL,
    stock INT DEFAULT 0,
    imagen_url VARCHAR(500),
    categoria_id INT NOT NULL,
    usuario_id INT NOT NULL, -- El vendedor
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (categoria_id) REFERENCES categorias(id),
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
);

-- Tabla ordenes
CREATE TABLE IF NOT EXISTS ordenes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_id INT NOT NULL, -- El comprador
    fecha_orden DATETIME DEFAULT CURRENT_TIMESTAMP,
    total DECIMAL(10,2) NOT NULL,
    estado ENUM('pendiente', 'procesando', 'completada', 'cancelada') DEFAULT 'pendiente',
    direccion_envio TEXT,
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
);

-- Tabla detalle_orden
CREATE TABLE IF NOT EXISTS detalle_orden (
    id INT PRIMARY KEY AUTO_INCREMENT,
    orden_id INT NOT NULL,
    producto_id INT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (orden_id) REFERENCES ordenes(id) ON DELETE CASCADE,
    FOREIGN KEY (producto_id) REFERENCES productos(id)
);

ALTER TABLE productos
CHANGE COLUMN categoria_id CategoriaId INT NOT NULL;

ALTER TABLE productos
CHANGE COLUMN usuario_id UsuarioId INT NOT NULL;

ALTER TABLE productos
CHANGE COLUMN fecha_creacion FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP;

ALTER TABLE productos
CHANGE COLUMN categoria_id CategoriaId INT NOT NULL,
CHANGE COLUMN usuario_id UsuarioId INT NOT NULL;

describe productos


ALTER TABLE productos
  CHANGE COLUMN categoria_id CategoriaId INT NOT NULL,
  CHANGE COLUMN usuario_id UsuarioId INT NOT NULL,
  CHANGE COLUMN fecha_creacion FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP;


ALTER TABLE usuarios MODIFY Apellido VARCHAR(255) NULL;


