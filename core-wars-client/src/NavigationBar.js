import React from "react";
import { Link } from "react-router-dom";



class NavigationBar extends React.Component {
    onLogoutLinkClicked(event){
        this.props.logoutAction()
        event.preventDefault()
    }


    renderNavigationLinks(){
        const navigationLinks = []

        if (this.props.loggedIn) {
            const dashboardLink = (<Link className="nav-bar-link nav-left-aligned" key="dashboard" to="/dashboard"> My robots </Link>)
            navigationLinks.push(dashboardLink)
        } else {
            const createAccountLink = (<Link className="nav-bar-link nav-left-aligned" key="create-account" to="/createAccount"> Create account </Link>)
            navigationLinks.push(createAccountLink)
        }

        const leaderboardLink = (<Link className="nav-bar-link nav-left-aligned" key="leaderboard" to="/leaderboard"> Leaderboard </Link>)
        navigationLinks.push(leaderboardLink)

        return navigationLinks
    }

    renderAccountControlLink(){
        if (!this.props.loggedIn) {
            return (<Link className="nav-bar-link nav-right-aligned" key="login" to="/login"> Log in </Link>)
        } else {
            return (<Link className="nav-bar-link nav-right-aligned" key="logout" to="/logout" onClick={this.onLogoutLinkClicked.bind(this)}> Log out </Link>)
        }
    }

    render() {

        let username = ''
        if(this.props.user != null) username = this.props.user.email

        return (
            <div className="navigation-bar background-bar">
                <ul className="nav-bar-list">
                    {this.renderNavigationLinks()}
                    {this.renderAccountControlLink()}
                    <span className="nav-bar-username">{username}</span>
                </ul>
            </div>
        )
    }
}

export default NavigationBar