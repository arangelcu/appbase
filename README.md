## InterviewApp - Monitoring System with Docker and Kubernetes

### Description

AppBase is a .NET application with a complete monitoring stack including:
- **Main application** (.NET with Entity Framework)
- **Database** (PostgreSQL with PostGIS)
- **Monitoring system** (Prometheus + Grafana)

### Prerequisites

- Docker and Docker Compose (for development)
- Kubernetes cluster (minikube, kind, or production cluster) for production deployment
- kubectl configured (for Kubernetes deployment)

### Quick Deployment

#### Docker Compose (Recommended for development)

```bash
# Clone the repository
git clone https://github.com/arangelcu/appbase.git
cd .\appbase\

# Build and start services
docker-compose up -d

# Check service status
docker-compose ps
```

#### Docker Compose >> Services and Ports
``` 
Service	        Local URL               Port	Description
PostgreSQL      localhost:50432	        50432	Database with PostGIS
AppBase     	http://localhost:8000	8000	Main .NET application

                ********PLUS*********
                
Prometheus      http://localhost:8001	8001	Monitoring and metrics system
Grafana	        http://localhost:8002	8002	Dashboards and visualizations
```

#### Kubernetes (Recommended for production)
```bash 
# Clone the repository
git clone https://github.com/arangelcu/appbase.git
cd appbase

# Apply Kubernetes manifests
kubectl apply -f k8s_postgres.yml
kubectl apply -f k8s_backend.yml
kubectl apply -f k8s_prometheus.yml
kubectl apply -f k8s_grafana.yml

# Verify pods are running
kubectl get pods --watch

# Alternatively, apply all YAML files in the directory
kubectl apply -f ./
```
#### Kubernetes >> Services and Ports (Kubernetes)
``` 
Service	        Local URL               Port	Description
PostgreSQL      localhost:30432	        30432	Database with PostGIS
AppBase     	http://localhost:30080	30080	Main .NET application

                ********PLUS*********
                
Prometheus      http://localhost:30081	30081	Monitoring and metrics system
Grafana	        http://localhost:30082	30082	Dashboards and visualizations
```
#### Configurations
``` 
PostgreSQL:                  Grafana:
Username: postgres           Username: admin
Password: postgres           Password: admin
Database: appbasedb
``` 

#### Documentation
``` 
To view the OpenAPI documentation, enable Development mode 

- ASPNETCORE_ENVIRONMENT=Development 
    --> ( file compose.yml for docker )
    --> ( file k8s_backend.yml for kubernetes )

and you can access it at the following URL:
    -->http://localhost:8000/docs/index.html (docker)
    -->http://localhost:30080/docs/index.html (kubernetes)
``` 
