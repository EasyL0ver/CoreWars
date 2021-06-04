import React from "react";
import axios from "axios";

import CompetitorButton from './CompetitorButton'
import UserStore from './UserStore'
import CompetitorEditView from "./CompetitorEditView";

class Dashboard extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            competitors: [],
            selectedBtnId: null,
            categories: [],
        }

        this.addCompetitor = this.addCompetitor.bind(this)
        this.editCompetitor = this.editCompetitor.bind(this)
    }

    componentDidMount() {
        this.loadCompetitors();
        this.getCompetitionNames();
    }

    async getCompetitionNames() {
        try {
            const response = await axios.get(
                "http://localhost:5000/competitions",
                { "Content-Type": "application/json" }
            );
            this.setState({
                ...this.state,
                categories: response.data,
            });

        } catch (err) {
            console.warn(err);
        }
    }

    async loadCompetitors() {
        try {
            let token = UserStore.token;
            const response = await axios.get(
                "http://localhost:5000/Competitors/",
                {
                    "Content-Type": "application/json",
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                }
            );

            this.setState({
                ...this.state,
                competitors: response.data
            });
        } catch (err) {
            console.warn(err);
        }
    }

    async addCompetitor(competitor){
        try {
            let token = UserStore.token;
            const response = await axios.post(
                "http://localhost:5000/Competitors/",
                competitor,
                {
                    "Content-Type": "application/json",
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                }
            );

            this.loadCompetitors();
        } catch (err) {
            console.warn(err);
        }
    }

    async editCompetitor(competitor, competitorId){
        try {
            let token = UserStore.token;
            const response = await axios.put(
                "http://localhost:5000/Competitors?editedCompetitorId=" + competitorId,
                competitor,
                {
                    "Content-Type": "application/json",
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                }
            );

            this.loadCompetitors();
        } catch (err) {
            console.warn(err);
        }
    }

    onButtonEdit(buttonId) {
        console.log(buttonId)
        this.setState({
            ...this.state,
            selectedBtnId: buttonId,
        });
    }

    onButtonAdd() {
        this.setState({
            ...this.state,
            selectedBtnId: null,
        });

    }

    render() {
        const buttons = this.state.competitors.map((competitor) => {
            let competitorSelected = competitor.id == this.state.selectedBtnId
            return (
                <CompetitorButton
                    alias={competitor.alias}
                    competitionName={competitor.competition}
                    scriptingLanguage={competitor.language}
                    id={competitor.id}
                    key={competitor.id}
                    gamesPlayed={competitor.gamesPlayed}
                    gamesWon={competitor.gamesWon}
                    status={competitor.status}
                    onClicked={this.onButtonEdit.bind(this)}
                    highlight={competitorSelected}
                />
            )
        });

        if (this.state.selectedBtnId == null) {
            const categories = this.state.categories.map(x => new Object({value: x.name, label: x.name}))
            return (
                <div>
                    {buttons}
                    <button onClick={this.onButtonAdd.bind(this)}> DODAJ ! (podswietlony)</button>
                    <br></br>
                    <br></br>
                    <CompetitorEditView 
                        alias={""}
                        code={""}
                        categories={categories}
                        submit={this.addCompetitor}/>
                </div>
            );
        }

        const editedCompetitor = this.state.competitors
            .find((competitor) => competitor.id === this.state.selectedBtnId)


        return (
            <div>
                {buttons} 
                <button onClick={this.onButtonAdd.bind(this)}> DODAJ ! </button>
                <br></br>
                <br></br>
                <CompetitorEditView
                    alias={editedCompetitor.alias}
                    code={editedCompetitor.code}
                    id={editedCompetitor.id}
                    submit={(competitor) => this.editCompetitor(competitor, editedCompetitor.id)}
                    categories={[{value:editedCompetitor.competition, label:editedCompetitor.competition}]} />
            </div>
        );
    }

}

export default Dashboard;