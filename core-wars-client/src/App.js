import './App.css';
import React from "react";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";

import LoginView from './LoginView'
import Dashboard from './Dashboard'
import Leaderboard from './Leaderboard';
import NavigationBar from './NavigationBar'
import Footer from './Footer'
import CreateAccountView from './CreateAccountView'


class App extends React.Component {
  constructor(props) {
    super(props)

    this.state = {
      user: this.storageLoadUser(),
      token: sessionStorage.getItem("token"),
      loggedIn: sessionStorage.getItem("loggedIn") || false
    }

    this.loginUser = this.loginUser.bind(this)
    this.logoutUser = this.logoutUser.bind(this)
  }

  storageLoadUser(){
    const id = sessionStorage.getItem("user.id")
    const email = sessionStorage.getItem("user.email")

    if(email == null || id == null)
      return null

    return {
      id: id,
      email: email
    }
  }

  loginUser(user, token) {
    console.log("ZALOGOWALES SIE GOSCIU")

    this.setState({
      ...this.state,
      user: user,
      token: token,
      loggedIn: true
    });

    this.persistenceWrite()
  }

  logoutUser() {
    this.setState({
      ...this.state,
      user: null,
      token: null,
      loggedIn: false
    });

    this.persistenceWrite()
  }

  persistenceWrite() {
    sessionStorage.setItem("token", this.state.token)
    sessionStorage.setItem("user.email", this.state.user.email)
    sessionStorage.setItem("user.id", this.state.user.id)
    sessionStorage.setItem("loggedIn", this.state.loggedIn)
  }

  persistenceRead() {
    this.setState({
      ...this.state,
      user: {
        id: sessionStorage.getItem("user.id"),
        email: sessionStorage.getItem("user.email")
      },
      token: sessionStorage.getItem("token"),
      loggedIn: sessionStorage.getItem("loggedIn")
    });
  }

  render() {

    const routes = []

    if (this.state.loggedIn) {
      routes.push((
        <Route path="/dashboard" key="dashboard">
          <Dashboard token={this.state.token} />
        </Route>
      ))
    } else {
      routes.push((
        <Route path="/login" key="login">
          <LoginView loginAction={this.loginUser} />
        </Route>
      ))
      routes.push((
        <Route path="/createAccount" key="createAccount" >
          <CreateAccountView />
        </Route>
      ))
    }

    return (
      <Router>
        <div>
          <NavigationBar loggedIn={this.state.loggedIn} user={this.state.user} logoutAction={this.logoutUser}/>
          {/* <AccountControl user={this.state.user} logoutAction={this.logoutUser}/> */}
          <div>
            <Switch>
              {routes}
              <Route path="/leaderboard">
                <Leaderboard user={this.state.user}/>
              </Route>
              <Route path="/test">
                TEST
              </Route>
            </Switch>
          </div>
          <Footer className="footer"/>
        </div>
      </Router>
    );
  }
}

export default App;
