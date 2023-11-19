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
                echo "Så afleverer vi!"
            }
        }
        stage("Deploy"){
            steps {                        
                bat "docker compose up --build"}
                }
            }
    }