pipeline{
    agent any
    triggers {
        pollSCM("* * * * *") 
    }
    stages{
        stage("Build"){
            steps {
                bat "docker-compose build"
            }
        }
        stage("Test"){
            steps {
                bat "Running tests..."
            }
        }
        stage("Deliver"){
            steps {
                withCredentials([usernamePassword(credentialsId: 'DockerHub', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                    bat 'echo %DOCKER_PASSWORD%|docker login --username %DOCKER_USERNAME% --password-stdin'
                    bat "docker-compose push"
                }
            }
        }
        stage("Deploy"){
            steps {                        
                bat "docker-compose -f docker-compose.yml up -d "
            }
        }
    }
}
