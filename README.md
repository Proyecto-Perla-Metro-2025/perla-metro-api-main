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
| `POST` | `/api/Auth/login` | Iniciar sesión y obtener JWT | No requerida |
| `POST` | `/api/Auth/Register` | Registrar nuevo ususario | No requerida |
| `GET` | `/api/Auth/GetAll` | Listado de todos los usuarios | JWT + Admin |
| `GET` | `/api/Auth/GetUser{id}` | Obtener un usuario por id  | No requerida |
| `GET` | `/api/Auth/UserFilter` | Listado de usuarios aplicando un filtro| JWT + Admin |
| `Put` | `/api/Auth/update-user` | Actualiza la información de un usuario | JWT |
| `Put` | `/api/Auth/enable-disable/{id}` | Permite actualizar el estado de un usuario, es decir activar o desactivar la cuenta de un usuario | JWT + Admin |

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
| `GET`    | `/api/Station/{id}`| Obtener estación por ID          | No Requerida   |
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

### Ejemplos de uso Ticket-Service (Postman):

**1. GET:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Ticket
Response (200 OK):
{
    "data": [
        {
            "id": "7692ea47-3454-4079-979f-fe6b95b191d2",
            "passengerId": "raul",
            "createdAt": "2025-09-27T23:42:37.5060526-03:00",
            "ticketType": "vuelta",
            "ticketStatus": "caducado",
            "amount": 77777777
        }
    ]
    "message": "Tickets retrieved successfully",
    "success": true
}
```

**2. POST:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Ticket
Body (JSON):
{
    "passengerId":"550e8400-e29b-41d4-a716-446655440000",
    "ticketType":"ida",
    "ticketStatus": "activo",
    "amount":1500
}
Response (200 OK):
{
    "data": {
        "id": "96b1531a-e129-449d-b65a-37c7235a31d1",
        "passengerId": "passenger-jhon77",
        "createdAt": "2025-09-28T21:13:54.0874644-03:00",
        "ticketType": "vuelta",
        "ticketStatus": "activo",
        "amount": 777
    },
    "message": "Ticket created successfully",
    "success": true
}
```

**3. GET/{id}:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Ticket/{id}
Response (200 OK):
{
    "data": {
        "id": "96b1531a-e129-449d-b65a-37c7235a31d1",
        "passengerId": "passenger-jhon77",
        "createdAt": "2025-09-28T21:13:54.0874644-03:00",
        "ticketType": "vuelta",
        "amount": 777
    },
    "message": "Ticket created successfully",
    "success": true
}
```

**4. PUT/{id}:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Ticket/{id} 
Body (JSON):
{
    "amount":7000
}
Response (200 OK):
{
    "data": {
        "id": "96b1531a-e129-449d-b65a-37c7235a31d1",
        "passengerId": "passenger-jhon77",
        "createdAt": "2025-09-28T21:17:10.8781898-03:00",
        "ticketType": "vuelta",
        "ticketStatus": "activo",
        "amount": 77777777
    },
    "message": "Ticket updated successfully",
    "success": true
}
```

**5. DELETE/{id}:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Ticket/{id}
Response (200 OK):
{
    "data": null,
    "message": "Ticket deleted successfully",
    "success": true
}
```

### Ejemplos de uso Station-Service (Postman):

**1. POST:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Station
Body (JSON):
{
  "Name": "Estación La Torre",
  "Location": "Antofagasta, calle La Torre",
  "StopType": "Origen"
}
Response (200 OK):
{
    "data": {
        "name": "Estación La Torre",
        "location": "Calle La Torre, Antofagasta",
        "stopType": "Origen"
    },
    "message": "Station created successfully",
    "success": true
}
```
**2. GET /api/Station:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Station
Response (200 OK):
{
    "data": [
        {
            "id": "2fb729b3-5af3-4fa6-b2ec-a2173f9adb30",
            "name": "Estación La Torre",
            "location": "Calle La Torre, Antofagasta",
            "stopType": "Origen",
            "status": "Active",
            "isActive": true
        }
    ],
    "message": "Stations retrieved successfully",
    "success": true
}
```
**2.1. GET /api/Station?name=La%20Torre:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Station?name=La%20Torre
Response (200 OK):
{
    "data": [
        {
            "id": "2fb729b3-5af3-4fa6-b2ec-a2173f9adb30",
            "name": "Estación La Torre",
            "location": "Calle La Torre, Antofagasta",
            "stopType": "Origen",
            "status": "Active",
            "isActive": true
        }
    ],
    "message": "Stations retrieved successfully",
    "success": true
}
```
**3. GET /api/Station/{Id}:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Station/2fb729b3-5af3-4fa6-b2ec-a2173f9adb30
Response (200 OK)
{
    "data": {
        "id": "2fb729b3-5af3-4fa6-b2ec-a2173f9adb30",
        "name": "Estación La Torre",
        "location": "Antofagasta, calle La Torre",
        "stopType": "Origen",
        "status": "Active",
        "isActive": true
    },
    "message": "Station retrieved successfully",
    "success": true
}
```
**4. PUT /api/Station/{Id}:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Station/2fb729b3-5af3-4fa6-b2ec-a2173f9adb30
Body (JSON):
{
  "Name": "Estación La Torre 2",
  "Location": "Antofagasta, calle La Torre",
  "StopType": "Origen"
}
Response (200 OK):
{
    "data": {
        "id": "2fb729b3-5af3-4fa6-b2ec-a2173f9adb30",
        "name": "Estación La Torre 2",
        "location": "Antofagasta, calle La Torre",
        "stopType": "Origen",
        "status": "Active",
        "isActive": true
    },
    "message": "Station updated successfully",
    "success": true
}
```
**4. DELETE /api/Station/{Id}:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Station/2fb729b3-5af3-4fa6-b2ec-a2173f9adb30
Response (200 OK):
{
    "data": {
        "id": "2fb729b3-5af3-4fa6-b2ec-a2173f9adb30",
        "name": "Estación La Torre 2",
        "location": "Calle La Torre, Antofagasta",
        "stopType": "Origen",
        "status": "Inactive",
        "isActive": false
    },
    "message": "Station deleted successfully",
    "success": true
}


```
### Ejemplos de uso Route-Service (Postman):
**1. GET /api/routes (Listar todas las rutas)**
* **Nota:** Este endpoint es público y no requiere autenticación.
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/routes
Response (200 OK):
{
    "data": [
        {
            "id": "6e6bebc7-fdff-4877-a77e-c513a581fcb4",
            "originStation": "Estación Central",
            "destinationStation": "Estación La Portada",
            "startTime": "2025-09-27T06:00:00",
            "endTime": "2025-09-27T07:00:00",
            "intermediateStops": [ "Estación Prat", "Estación Latorre" ],
            "isActive": true
        }
    ],
    "message": "Routes retrieved successfully",
    "success": true
}
```
**2. POST /api/routes (Crear una ruta)**
* **Nota:** Este endpoint es protegido y requiere un Bearer Token con rol de "Admin".
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/routes

{
    "originStation": "Estación Alfa",
    "destinationStation": "Estación Omega",
    "startTime": "2025-10-01T08:00:00Z",
    "endTime": "2025-10-01T09:30:00Z",
    "intermediateStops": [ "Estación Beta" ]
}
Response (201 Created):
{
"data": {
    "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
    "originStation": "Estación Alfa",
    "destinationStation": "Estación Omega",
    "startTime": "2025-10-01T08:00:00Z",
    "endTime": "2025-10-01T09:30:00Z",
    "intermediateStops": [ "Estación Beta" ],
    "isActive": true
    },
    "message": "Route created successfully",
    "success": true
}
```
**3. GET /api/routes/{id} (Obtener por ID)**
* **Nota:** Este endpoint es público.
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/routes/6e6bebc7-fdff-4877-a77e-c513a581fcb4
Response (200 OK):
{
"data": {
    "id": "6e6bebc7-fdff-4877-a77e-c513a581fcb4",
    "originStation": "Estación Central",
    "destinationStation": "Estación La Portada",
    "startTime": "2025-09-27T06:00:00",
    "endTime": "2025-09-27T07:00:00",
    "intermediateStops": [ "Estación Prat", "Estación Latorre" ],
    "isActive": true
    },
    "message": "Route retrieved successfully",
    "success": true
}
```
**4. PUT /api/routes/{id} (Actualizar una ruta)**
* **Nota:** Este endpoint es protegido y requiere un Bearer Token con rol de "Admin".
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/routes/a1b2c3d4-e5f6-7890-1234-567890abcdef
Body (JSON):
{
    "originStation": "Estación Alfa (Actualizada)",
    "destinationStation": "Estación Omega (Nueva)"
}
Response (200 OK):
{
    "data": null,
    "message": "Route updated successfully",
    "success": true
}
```
**5. DELETE /api/routes/{id} (Desactivar una ruta)**
* **Nota:** Este endpoint es protegido y requiere un Bearer Token con rol de "Admin".
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/routes/a1b2c3d4-e5f6-7890-1234-567890abcdef
Response (200 OK):
{
    "data": null,
    "message": "Route deactivated successfully",
    "success": true
}
```
### Ejemplos de uso Users-Service (Postman):
**1. POST /api/Auth/login:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Auth/login
Body (JSON):
{
  "email": "Admin@perlametro.cl",
  "password": "Password123+"
}
Response (200 OK):
{
    {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjA1Y2UxZjc4LTk5NjYtNGRlYy05NDdhLTEzZWU5MWRkMjViZSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6IkFkbWluQHBlcmxhbWV0cm8uY2wiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2dpdmVubmFtZSI6Ik5vbWJyZV9BZG1pbiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL3N1cm5hbWUiOiJBcGVsbGlkb19BZG1pbiIsImZ1bGxOYW1lIjoiTm9tYnJlX0FkbWluIEFwZWxsaWRvX0FkbWluIiwicmVnaXN0cmF0aW9uRGF0ZSI6IjIwMjUtMDktMjUiLCJqdGkiOiJmYmIwYmQ5OC1kMTQxLTRkNTYtYjA0Ny1mMWU4ZGJmN2RjYWQiLCJpYXQiOjE3NTkxMDYzNTQsImV4cCI6MTc1OTEwOTk1NCwiaXNzIjoiaHR0cHM6Ly9wZXJsYS1tZXRyby1hcGktbWFpbi5vbnJlbmRlci5jb20iLCJhdWQiOiJodHRwczovL3BlcmxhLW1ldHJvLWFwaS1tYWluLm9ucmVuZGVyLmNvbSJ9.fW4GuAgJgo3Z6jLuQKEM-_vc0W-zOIOEStEpoxBcl4g",
    "expiresAt": "2025-09-29T01:39:14.9533373Z",
    "user": {
        "id": "05ce1f78-9966-4dec-947a-13ee91dd25be",
        "email": "Admin@perlametro.cl",
        "role": "Admin",
        "fullName": "Nombre_Admin Apellido_Admin"
        }
    }
}
```

**2. POST /api/Auth/Register:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Auth/Register
Body (JSON):
{
  "name": "Juan",
  "surename": "Pohl",
  "email": "example@perlametro.cl",
  "password": "Password1234*"
}
Response (200 OK):
{
    {
    "id": "022a46f3-538e-4ccb-b68a-05cd9922d7fa",
    "email": "example@perlametro.cl",
    "name": "Juan",
    "sureName": "Pohl",
    "role": "User",
    "createdAt": "0001-01-01T00:00:00",
    "isActive": true
    }
}
```
**3. Get /api/Auth/GetAll:**
* **Nota:** Este endpoint es protegido y requiere un Bearer Token con rol de "Admin".
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Auth/GetAll
Response (200 OK):
{
    [
        {
            "id": "05ce1f78-9966-4dec-947a-13ee91dd25be",
            "fullname": "Nombre_Admin Apellido_Admin",
            "email": "Admin@perlametro.cl",
            "isActive": true,
            "registrationDate": "2025-09-25"
        },
        ...
        {
            "id": "022a46f3-538e-4ccb-b68a-05cd9922d7fa",
            "fullname": "Juan Pohl",
            "email": "example@perlametro.cl",
            "isActive": true,
            "registrationDate": "2025-09-29"
        }
    ]
}
```
**4. Get /api/Auth/GetUser/{id}:**
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Auth/GetUser?id=05ce1f78-9966-4dec-947a-13ee91dd25be
Response (200 OK):
{
    "id": "05ce1f78-9966-4dec-947a-13ee91dd25be",
    "fullname": "Nombre_Admin Apellido_Admin",
    "email": "Admin@perlametro.cl",
    "isActive": true,
    "registrationDate": "2025-09-25"
}
```
**5. Get /api/Auth/UserFilter:**
* **Nota:** Este endpoint es protegido y requiere un Bearer Token con rol de "Admin".
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Auth/UserFilter?isActive=false
Response (200 OK):
{
    [
        {
            "id": "f8d87978-173a-4a30-9ad2-ade53f2f5095",
            "fullname": "Adrian Morgan",
            "email": "sWhxRyHcK@perlametro.cl",
            "isActive": false,
            "registrationDate": "2025-09-25"
        },
        {
            "id": "3e72480f-b4a8-49f1-9da8-940101ceeb5b",
            "fullname": "stdasd string",
            "email": "string111@perlametro.cl",
            "isActive": false,
            "registrationDate": "2025-09-27"
        }
    ]
}
```
**6. PUT /api/Auth/update-user:**
* **Nota:** Este endpoint es protegido y requiere un Bearer Token.
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/Auth/update-user
Body (JSON):
{
  "password": "Password1234+"
}
Response (200 OK):
{
    {
    "id": "022a46f3-538e-4ccb-b68a-05cd9922d7fa",
    "email": "example@perlametro.cl",
    "name": "Juan",
    "sureName": "Pohl",
    "role": "User",
    "createdAt": "0001-01-01T00:00:00",
    "isActive": true
    }
}
```
**7. Get /api/Auth/enable-disable/{id}:**
* **Nota:** Este endpoint es protegido y requiere un Bearer Token con rol de "Admin".
```
URL: https://perla-metro-api-main-ohy4.onrender.com/api/api/Auth/enable-disable?Id=022a46f3-538e-4ccb-b68a-05cd9922d7fa
Response (200 OK):
{

}
```

### Observaciones / Consideraciones

- La base de datos para station-service se encuentra desplegada en Railway en modo Serverless, lo que significa que, en caso de bajo tráfico, se apagará automáticamente y se encenderá al recibir la primera solicitud. Por lo tanto, se solicita reintentar las primeras solicitudes realizadas.

- Los servicios se encuentran desplegados en el plan gratuito de Render, siendo estos suspendidos automáticamente tras 15 minutos de actividad ("Se duermen"), por lo tanto, se solicita reintentar las primeras solicitudes realizadas ("Despertarlo")

- El sistema Health Check de Render frecuentemente hace peticiones URL a la raíz del Gateway (Ejemplo: GET/), esto genera errores de tipo *UnableToFindDownstreamRouteError* en los logs de API Main, estos errores son normales y esperados, no indican un fallo en la aplicación.

- Durante el primer despliegue es posible que los logs muestren un mensaje como *New primary port detected... Restarting deploy....*, es un comportamiento normal y no requiere ninguna acción por parte del usuario.

### URL de Despliegue Directo

El despliegue de este servicio se encuentra en `https://perla-metro-api-main-ohy4.onrender.com/`.
