import React from "react";
import { Link } from "react-router-dom";


class NavigationBar extends React.Component {
    render() {

        const navigationLinks = []

        if (this.props.loggedIn) {
            const dashboardLink = (<Link key="dashboard" to="/dashboard"> DASHBOARD </Link>)
            navigationLinks.push(dashboardLink)
        } else {
            const createAccountLink = (<Link key="create-account" to="/createAccount"> CREATE ACCOUNT </Link>)
            navigationLinks.push(createAccountLink)
        }

        const leaderboardLink = (<Link key="leaderboard" to="/leaderboard"> LEADERBOARD </Link>)
        navigationLinks.push(leaderboardLink)

        return (
            <div className="navigation-bar">
                {navigationLinks}
            </div>
        )
    }
}

export default NavigationBar