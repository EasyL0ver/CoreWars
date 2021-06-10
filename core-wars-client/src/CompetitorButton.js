import './CompetitorButton.css';
import React from "react";


class CompetitorButton extends React.Component {
    constructor(props) {
        super(props)

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
                <span> {this.props.competitor.alias} </span>
                <span> {this.props.competitor.competitionName} </span>
                <span> {this.props.competitor.scriptingLanguage} </span>
                <span> P: {this.props.competitor.gamesPlayed} </span>
                <span> W: {this.props.competitor.gamesWon} </span>
                <span> STATUS: {this.props.competitor.status} </span>
                <button onClick={this.onButtonClicked.bind(this)}> {hightlightedIndicator} </button>
            </div>
        );
    }

}

export default CompetitorButton;