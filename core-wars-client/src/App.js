import './App.css';
import React from "react";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";

import LoginView from './LoginView'
import Dashboard from './Dashboard'
import Leaderboard from './Leaderboard';
import UserStore from './UserStore'

class App extends React.Component {
  render() {
    UserStore.refresh()
    return (
      <Router>
        <div className="container-fluid">
          {/* <Navigation /> */}
          <div className="jumbotron">
            <Switch>
              <Route path="/login" exact component={LoginView} />
              <Route path="/dashboard" exact component={Dashboard} />
              <Route path="/leaderboard" exact component={Leaderboard} />
            </Switch>
          </div>
        </div>
      </Router>
    );
  }
}

export default App;
