import './CompetitorButton.css';
import React from "react";


class CompetitorButton extends React.Component {
    constructor(props) {
        super(props)

    }

    onButtonClicked() {
        this.props.onClicked(this.props.id)
    }

    render() {
        return (
            <div onClick={this.onButtonClicked.bind(this)}
                className={"highlighted-" + this.props.highlight + " competitor-button dashboard-burger-choice"} >
                <span className="alias"> {this.props.competitor.alias} </span>
                <div className="results">
                    <span style={{ color: 'green' }}> {this.props.competitor.gamesWon}</span>
                    <span>/</span>
                    <span style={{ color: 'brown' }}>{this.props.competitor.gamesPlayed} </span>
                </div>
                <div className='status-dot-container'>
                    <div className={'status-dot status-' + this.props.competitor.status}></div>
                </div>
            </div>
        );
    }

}

export default CompetitorButton;