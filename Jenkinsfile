pipeline{
    agent any
    triggers {
        pollSCM("* * * * *")
    }
    stages{
        stage("Build"){
            steps {
                echo "Så bygger vi!"
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
    
    }
}