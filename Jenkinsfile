pipeline {
  agent any
  stages {
    stage('Build solution') {
      steps {
        sleep 10
      }
    }
    stage('Unit tests Windows') {
      steps {
        parallel(
          "Unit tests Windows": {
            sleep 10
            
          },
          "Unit tests Linux": {
            sleep 10
            
          },
          "Unit tests Mac": {
            sleep 10
            
          }
        )
      }
    }
    stage('Selenium tests Firefox') {
      steps {
        parallel(
          "Selenium tests Firefox": {
            sleep 10
            
          },
          "Selenium tests Chrome": {
            sleep 10
            
          }
        )
      }
    }
    stage('Publish results') {
      steps {
        sleep 10
      }
    }
  }
}