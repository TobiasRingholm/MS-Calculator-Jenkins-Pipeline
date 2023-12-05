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
                echo "Running tests..."
            }
        }
        stage("Deliver"){
            steps {
                withCredentials([usernamePassword(credentialsId: 'DockerHub', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                    bat 'echo %DOCKER_PASSWORD%|docker login --username %DOCKER_USERNAME% --password-stdin'
                }
            }
        }
        stage("Deploy"){
            steps {                        
                echo "Deploying application..."
            }
        }
    }
}
