import './CompetitorButton.css';
import React from "react";
import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr"

import UserStore from './UserStore'


class CompetitorButton extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            competitorState: {
                gamesPlayed: props.gamesPlayed,
                wins: props.gamesWon,
                state: props.status
            }
        }
    }

    componentDidMount() {
        this.setUpNotifications()
    }

    async start() {
        try {
            await this.connection.start();
            console.log("SignalR Connected.");
            this.connection.invoke("Register", null, this.props.id).catch(function (err) {
                return console.error(err.toString());
            });
        } catch (err) {
            console.log(err);
            setTimeout(this.start, 5000);
        }
    };

    setUpNotifications() {
        this.connection = new HubConnectionBuilder()
            .withUrl("http://localhost:5000/competitor/status", {accessTokenFactory: () => UserStore.token})
            .configureLogging(LogLevel.Information)
            .build();

        this.connection.on("Status", (message) => {
            console.log(message)
            this.setState({
                ...this.state,
                competitorState: message,
            });

        });

        this.connection.onclose(this.start);
        this.start();
    }

    onButtonClicked(){
        this.props.onClicked(this.props.id)
    }

    render() {
        let hightlightedIndicator = "EDYTUJ"

        if(this.props.highlight)
            hightlightedIndicator = "EDYTUJESZ!"

        return (
            <div >
                <span> {this.props.alias} </span>
                <span> {this.props.competitionName} </span>
                <span> {this.props.scriptingLanguage} </span>
                <span> P: {this.state.competitorState.gamesPlayed} </span>
                <span> W: {this.state.competitorState.wins} </span>
                <span> STATUS: {this.state.competitorState.state} </span>
                <button onClick={this.onButtonClicked.bind(this)}> {hightlightedIndicator} </button>
            </div>
        );
    }

}

export default CompetitorButton;