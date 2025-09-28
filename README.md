# perla-metro-api-main

API Gateway para el sistema de transporte subterráneo de Antofagasta. Implementa un patrón de API Gateway utilizando Ocelot para orquestar la comunicación con los microservicios del sistema Perla Metro, proporcionando autenticación centralizada y proxy inteligente.

## Arquitectura y Patrón de Diseño

### Arquitectura: Monolito Distribuido con SOA (Service-Oriented Architecture)

El Ticket Service implementa una arquitectura de capas (Layered Architecture) dentro del contexto SOA

```

```

### Patrones de Diseño Implementados:

1. **Dependency Injection:** Inyección de dependencias
2. **Data Transfer Object (DTO):** Transferencia de datos entre capas
3. **API Gateway Pattern:** Punto único de entrada para todos los servicios
4. **Proxy Pattern:** Ocelot actúa como proxy reverso
5. **Authentication Pattern:** JWT centralizado para todos los servicios

## Tecnologías Utilizadas

- **Framework:** ASP.NET Core 9.0 (Minimal API)
- **API Gateway:** Ocelot
- **Autenticación:** JWT Bearer Tokens
- **Proxy:** HTTP Client Factory

## Funcionalidades Principales

### Autenticación Centralizada
- Login único para todos los servicios
- Generación de tokens JWT
- Validación automática en todas las rutas

### API Gateway (Ocelot)
- Proxy automático a microservicios

### Servicios Integrados
- **User Service**: Gestión de usuarios y autenticación
- **Ticket Service**: Emisión y gestión de boletos
- **Route Service**: Rutas y horarios de transporte
- **Station Service**: Gestión de estaciones

## Endpoints Disponibles

### Autenticación (Controller)
| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| `POST` | `/api/auth/login` | Iniciar sesión y obtener JWT | No requerida |

### Servicios (Proxy vía Ocelot)
| Método   | Endpoint           | Descripción                      | Autenticación  |
| :------- | :----------------- | :------------------------------- | :------------- |
| `GET`    | `/api/Ticket`      | Listar tickets                   | JWT + Admin    |
| `POST`   | `/api/Ticket`      | Crear ticket                     | JWT + Admin    |
| `GET`    | `/api/Ticket/{id}` | Obtener ticket                   | JWT + Admin    |
| `PUT`    | `/api/Ticket/{id}` | Actualizar ticket                | JWT + Admin    |
| `DELETE` | `/api/Ticket/{id}` | Eliminar ticket                  | JWT + Admin    |
| `GET`    | `/api/routes`      | Listar todas las rutas           | No requerida   |
| `GET`    | `/api/routes/{id}` | Obtener ruta por ID              | No requerida   |
| `POST`   | `/api/routes`      | Crear nueva ruta                 | JWT + Admin    |
| `PUT`    | `/api/routes/{id}` | Actualizar ruta                  | JWT + Admin    |
| `DELETE` | `/api/routes/{id}` | Eliminar ruta (soft delete)      | JWT + Admin    |
| `GET`    | `/api/Station`     | Listar todas las estaciones      | JWT + Admin    |
| `POST`   | `/api/Station`     | Crear nueva estación             | JWT + Admin    |
| `GET`    | `/api/Station/{id}`| Obtener estación por ID          | -              |
| `PUT`    | `/api/Station/{id}`| Actualizar estación              | JWT + Admin    |
| `DELETE` | `/api/Station/{id}`| Eliminar estación (soft delete)  | JWT + Admin    |



## 🚀 Instalación y Configuración

### Requisitos Previos
- .NET 9.0 SDK
- Visual Studio Code o Visual Studio 2022

### 1. Clonar el Repositorio
```bash
git clone https://github.com/Proyecto-Perla-Metro-2025/perla-metro-api-main.git

cd perla-metro-api-main
```

### 3.  Instalar Dependencias
```bash
dotnet restore
```
### 4. Ejecutar el Proyecto
```bash
cd ApiMain

dotnet run
```

