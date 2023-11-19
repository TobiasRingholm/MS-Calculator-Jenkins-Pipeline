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
                bat "docker compose push"
            }
        }
        stage("Deploy"){
            steps {                        
                bat "docker compose up"}
                }
            }
    }