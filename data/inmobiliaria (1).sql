-- phpMyAdmin SQL Dump
-- version 5.2.2
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: Mar 25, 2025 at 06:44 PM
-- Server version: 8.0.30
-- PHP Version: 8.1.10

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Table structure for table `contrato`
--

CREATE TABLE `contrato` (
  `id_contrato` int NOT NULL,
  `id_inquilino` int NOT NULL,
  `id_inmueble` int NOT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date NOT NULL,
  `monto_mensual` decimal(10,2) NOT NULL,
  `estado` enum('vigente','terminado') DEFAULT 'vigente'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `contrato`
--

INSERT INTO `contrato` (`id_contrato`, `id_inquilino`, `id_inmueble`, `fecha_inicio`, `fecha_fin`, `monto_mensual`, `estado`) VALUES
(1, 1, 1, '2023-01-01', '2023-12-31', 50000.00, 'vigente'),
(2, 2, 3, '2023-02-01', '2023-11-30', 40000.00, 'terminado'),
(3, 3, 4, '2023-03-01', '2023-08-31', 60000.00, 'vigente');

-- --------------------------------------------------------

--
-- Table structure for table `inmueble`
--

CREATE TABLE `inmueble` (
  `id_inmueble` int NOT NULL,
  `id_propietario` int NOT NULL,
  `direccion` varchar(200) NOT NULL,
  `uso` enum('comercial','residencial') NOT NULL,
  `tipo` enum('local','deposito','casa','departamento') NOT NULL,
  `ambientes` int NOT NULL,
  `precio` decimal(10,2) NOT NULL,
  `estado` enum('disponible','suspendido','ocupado') DEFAULT 'disponible'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `inmueble`
--

INSERT INTO `inmueble` (`id_inmueble`, `id_propietario`, `direccion`, `uso`, `tipo`, `ambientes`, `precio`, `estado`) VALUES
(1, 1, 'Calle Falsa 123', 'residencial', 'casa', 3, 50000.00, 'disponible'),
(2, 1, 'Avenida Siempre Viva 456', 'comercial', 'local', 1, 80000.00, 'suspendido'),
(3, 2, 'Ruta 9 Km 10', 'residencial', 'departamento', 2, 40000.00, 'ocupado'),
(4, 3, 'Pasaje Secreto 789', 'comercial', 'deposito', 0, 60000.00, 'disponible');

-- --------------------------------------------------------

--
-- Table structure for table `inquilino`
--

CREATE TABLE `inquilino` (
  `id_inquilino` int NOT NULL,
  `dni` varchar(10) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `telefono` varchar(15) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `inquilino`
--

INSERT INTO `inquilino` (`id_inquilino`, `dni`, `nombre`, `apellido`, `telefono`, `email`) VALUES
(1, '34567890', 'Laura Martínez', '', '1134567890', 'laura.martinez@example.com'),
(2, '45678901', 'Roberto Fernández', '', '1145678901', 'roberto.fernandez@example.com'),
(3, '56789012', 'Ana Díaz', '', '1156789012', 'ana.diaz@example.com');

-- --------------------------------------------------------

--
-- Table structure for table `pago`
--

CREATE TABLE `pago` (
  `id_pago` int NOT NULL,
  `id_contrato` int NOT NULL,
  `fecha_pago` date NOT NULL,
  `importe` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `pago`
--

INSERT INTO `pago` (`id_pago`, `id_contrato`, `fecha_pago`, `importe`) VALUES
(1, 1, '2023-01-15', 50000.00),
(2, 1, '2023-02-15', 50000.00),
(3, 1, '2023-03-15', 50000.00),
(4, 2, '2023-02-15', 40000.00),
(5, 3, '2023-03-15', 60000.00),
(6, 3, '2023-04-15', 60000.00);

-- --------------------------------------------------------

--
-- Table structure for table `propietario`
--

CREATE TABLE `propietario` (
  `id_propietario` int NOT NULL,
  `dni` varchar(10) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `telefono` varchar(15) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `propietario`
--

INSERT INTO `propietario` (`id_propietario`, `dni`, `nombre`, `apellido`, `telefono`, `email`) VALUES
(1, '123', 'Juan ', 'Perez', '1112345678', 'juan.perez@example.com'),
(2, '456', 'María ', 'Lopez', '1187654321', 'maria.lopez@example.com'),
(3, '789', 'Carlos', 'Garcia', '1123456789', 'carlos.garcia@example.com');

-- --------------------------------------------------------

--
-- Table structure for table `usuario`
--

CREATE TABLE `usuario` (
  `id_usuario` int NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `rol` enum('Admin','Empleado') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `usuario`
--

INSERT INTO `usuario` (`id_usuario`, `username`, `password`, `rol`) VALUES
(1, 'fran', '123', 'Empleado'),
(2, 'admin', '123', 'Admin');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`id_contrato`),
  ADD KEY `id_inquilino` (`id_inquilino`),
  ADD KEY `id_inmueble` (`id_inmueble`);

--
-- Indexes for table `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`id_inmueble`),
  ADD KEY `id_propietario` (`id_propietario`);

--
-- Indexes for table `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`id_inquilino`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indexes for table `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`id_pago`),
  ADD KEY `id_contrato` (`id_contrato`);

--
-- Indexes for table `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`id_propietario`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indexes for table `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id_usuario`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `contrato`
--
ALTER TABLE `contrato`
  MODIFY `id_contrato` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id_inmueble` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id_inquilino` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `pago`
--
ALTER TABLE `pago`
  MODIFY `id_pago` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id_propietario` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id_usuario` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilino` (`id_inquilino`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id_inmueble`);

--
-- Constraints for table `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id_propietario`);

--
-- Constraints for table `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id_contrato`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
