pipeline{
    agent any
    triggers {
        pollSCM("* * * * *")
    }
    stages{
        stage("Build"){
            steps {
                bat "docker compose build"
            }
        }
        stage("Test"){
            steps {
                echo "Så tester vi!"
            }
        }
        stage("Deliver"){
            steps {
                withCredentials([usernamePassword(credentialsId: 'docker-hub-credentials-id', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                bat 'echo $DOCKER_PASSWORD | docker login --username $DOCKER_USERNAME --password-stdin'
            }
        }
        stage("Deploy"){
            steps {                        
                echo "Så deployer vi!"}
                }
            }
    post { 
        success { 
            bat "docker compose up --build"}  
        }
}