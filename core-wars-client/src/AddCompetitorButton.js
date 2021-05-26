import './CompetitorButton.css';
import React from "react";


class AddCompetitionButton extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            competitorState: {
                gamesPlayed:0
            }
        }
    }

    componentDidMount() {
    }

    onButtonClicked(){
        this.props.onClicked(this)
    }

    render() {
        let hightlightedIndicator = "niepodswietlony"

        if(this.props.highlight)
            hightlightedIndicator = "podswietlony"

        return (
            <div >
                <span> {this.props.competitionName} </span>
                <span> HIGHTLIGHTED: {hightlightedIndicator} </span>
                <button onClick={this.onButtonClicked.bind(this)}> EDYTUJ! </button>
            </div>
        );
    }

}

export default AddCompetitionButton;