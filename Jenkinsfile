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
                echo "Følgende virker ikke.... withCredentials([usernamePassword(credentialsId: 'DockerHub', usernameVariable: 'USERNAME', passwordVariable: 'PASSWORD')]){
                    bat "echo %PASSWORD% | docker login -u %USERNAME% --password-stdin"
                    bat "docker compose push""
            }
        }
        stage("Deploy"){
            steps {                        
                bat "docker compose up --build"}
                }
            }
    }