# perla-metro-api-main

API Gateway para el sistema de transporte subterráneo de Antofagasta. Implementa un patrón de API Gateway utilizando Ocelot para orquestar la comunicación con los microservicios del sistema Perla Metro, proporcionando autenticación centralizada y proxy inteligente.

## Arquitectura y Patrón de Diseño

### Arquitectura: Gateway Híbrido en un Monolito Distribuido con SOA (Service-Oriented Architecture)

El proyecto implementa una arquitectura de **Monolito Distribuido** bajo un enfoque **SOA**. La pieza central es la `API Main`, que no actúa como un simple gateway, sino que desempeña un rol dual y estratégico:

1.  **API Gateway (con Ocelot):** Para los microservicios de negocio (`Routes`, `Tickets`, `Stations`), la `API Main` funciona como un **API Gateway** puro. Utiliza **Ocelot** para enrutar las peticiones externas hacia el servicio interno correspondiente. Esto centraliza la gestión de rutas, la seguridad y simplifica la comunicación desde el cliente.

2.  **Fachada de Autenticación (con Controller local):** Para la gestión de usuarios, la `API Main` actúa como una **Fachada (Façade)**. En lugar de redirigir el tráfico, expone su propio `AuthController`. Este controlador se comunica internamente mediante `HttpClient` con el **Users Service** (que es un monolito independiente). Esta decisión de diseño centraliza la lógica de autenticación y la generación de tokens JWT en el gateway, proveyendo una capa de seguridad robusta y desacoplando al cliente de la implementación interna del servicio de usuarios.
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

