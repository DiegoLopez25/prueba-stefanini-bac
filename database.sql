USE master; -- Se ejecuta en master para poder crear la DB

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CrearDBVentas]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_CrearDBVentas];
GO

CREATE PROCEDURE sp_CrearDBVentas
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @DBName NVARCHAR(100) = 'db_ventas';

    -- 1. Crear la base de datos si no existe
    IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = @DBName)
    BEGIN
        SET @SQL = 'CREATE DATABASE ' + @DBName + ';';
        EXEC sp_executesql @SQL;
        PRINT 'Base de datos db_ventas creada.';
    END
    ELSE
    BEGIN
        PRINT 'La base de datos db_ventas ya existe.';
    END
    
    -- 2. Usar SQL dinámico para ejecutar comandos dentro de la nueva DB
    --    Se construye un script masivo y se ejecuta
    SET @SQL = '
    USE ' + @DBName + ';

    -- Crear tablas
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[tipo_usuario]'') AND type in (N''U''))
    BEGIN
        CREATE TABLE tipo_usuario (
            id_tipo INT IDENTITY(1,1) PRIMARY KEY,
            nombre VARCHAR(50) NOT NULL UNIQUE
        );
    END;

    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[productos]'') AND type in (N''U''))
    BEGIN
        CREATE TABLE productos (
            idpro INT IDENTITY(1,1) PRIMARY KEY,
            codigo VARCHAR(75) NOT NULL UNIQUE,
            producto VARCHAR(255) NOT NULL,
            precio DECIMAL(12,2) NOT NULL,
            delete_at DATETIME NULL
        );
    END;

    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[usuarios]'') AND type in (N''U''))
    BEGIN
        CREATE TABLE usuarios (
            id_usuario INT IDENTITY(1,1) PRIMARY KEY,
            nombre NVARCHAR(255) NOT NULL,
            usuario NVARCHAR(100) NOT NULL UNIQUE,
            password NVARCHAR(500) NOT NULL,
            id_tipo INT NOT NULL FOREIGN KEY REFERENCES tipo_usuario(id_tipo)
        );
    END;

    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[ventas]'') AND type in (N''U''))
    BEGIN
        CREATE TABLE ventas (
            idventa INT IDENTITY(1,1) PRIMARY KEY,
            fecha DATETIME NOT NULL DEFAULT GETDATE(),
            idvendedor INT NOT NULL FOREIGN KEY REFERENCES usuarios(id_usuario),
            total DECIMAL(10,2) NOT NULL
        );
    END;

    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[detalle_ventas]'') AND type in (N''U''))
    BEGIN
        CREATE TABLE detalle_ventas (
            idde INT IDENTITY(1,1) PRIMARY KEY,
            fecha DATETIME NOT NULL DEFAULT GETDATE(),
            idventa INT NOT NULL FOREIGN KEY REFERENCES ventas(idventa),
            idpro INT NOT NULL FOREIGN KEY REFERENCES productos(idpro),
            cantidad INT NOT NULL,
            precio DECIMAL(10,2) NOT NULL,
            iva DECIMAL(10,2) NOT NULL,
            total DECIMAL(10,2) NOT NULL
        );
    END;

    -- Insertar datos iniciales si no existen
    IF NOT EXISTS (SELECT 1 FROM tipo_usuario WHERE nombre = ''admin'')
    BEGIN
        INSERT INTO tipo_usuario (nombre) VALUES (''admin''), (''operador'');
    END;

    IF NOT EXISTS (SELECT 1 FROM usuarios WHERE usuario = ''admin'')
    BEGIN
        DECLARE @adminTipoId INT = (SELECT id_tipo FROM tipo_usuario WHERE nombre = ''admin'');
        DECLARE @operadorTipoId INT = (SELECT id_tipo FROM tipo_usuario WHERE nombre = ''operador'');

        INSERT INTO usuarios (nombre, usuario, password, id_tipo)
        VALUES 
        (''Administrador'', ''admin'', ''8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918'', @adminTipoId),
        (''Operador'', ''operador'', ''03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4'', @operadorTipoId);
    END;
    ';

    -- Ejecutar el script dinámico para la creación de tablas e inserciones
    EXEC sp_executesql @SQL;

    PRINT 'Esquema db_ventas y datos iniciales configurados correctamente.';

END;
GO