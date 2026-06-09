pipeline {
    agent any

    environment {
        IMAGE = "todo-backend:${BUILD_NUMBER}"
        CONTAINER = "todo-backend"
        NETWORK = "todo-network"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Create Network') {
            steps {
                bat "docker network create %NETWORK% 2>nul || exit 0"
            }
        }

        stage('Build Backend Image') {
            steps {
                bat "docker build -t %IMAGE% ."
                bat "docker tag %IMAGE% todo-backend:latest"
            }
        }

        stage('Deploy Backend') {
            steps {
                bat "docker rm -f %CONTAINER% 2>nul || exit 0"
                bat "docker run -d --name %CONTAINER% --network %NETWORK% -p 5000:8080 todo-backend:latest"
            }
        }
    }
}