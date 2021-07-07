import React from "react";
import axios from "axios";

import CompetitorButton from './CompetitorButton'
import CompetitorEditView from "./CompetitorEditView";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr"

import './Dashboard.css'
import './CompetitorButton.css'
import './LoginView.css'


class Dashboard extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            competitors: [],
            selectedBtnId: null,
            categories: [],
            addKey: Math.random()
        }

        this.addCompetitor = this.addCompetitor.bind(this)
        this.editCompetitor = this.editCompetitor.bind(this)
    }

    componentDidMount() {
        this.initializeCompetitors()
        this.getCompetitionNames()
    }

    async initializeCompetitors() {
        await this.loadCompetitors();
        //this.setUpNotifications();
    }

    async start() {
        try {
            await this.connection.start();
            console.log("SignalR Connected.");

            this.state.competitors.forEach(competitor => {
                this.connection.invoke("Register", null, competitor.id).catch(function (err) {
                    return console.error(err.toString());
                });
            })

        } catch (err) {
            console.log(err);
            setTimeout(this.start, 5000);
        }
    };

    setUpNotifications() {
        this.connection = new HubConnectionBuilder()
            .withUrl("http://localhost:5000/competitor/status", { accessTokenFactory: () => this.props.token })
            .configureLogging(LogLevel.Information)
            .build();

        this.connection.on("Status", (message) => {
            console.log(message)
            let newcompetitors = this.state.competitors
            let changedCompetitorIndex = newcompetitors.findIndex(x => x.id == message.competitorId)

            if (changedCompetitorIndex == -1)
                return

            newcompetitors[changedCompetitorIndex].status = message.state
            newcompetitors[changedCompetitorIndex].gamesPlayed = message.gamesPlayed
            newcompetitors[changedCompetitorIndex].gamesWon = message.wins
            newcompetitors[changedCompetitorIndex].exception = message.exceptionString
            this.setState({
                ...this.state,
                competitors: newcompetitors
            });

        });

        this.connection.onclose(this.start);
        this.start();
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
            const response = await axios.get(
                "http://localhost:5000/Competitors/",
                {
                    "Content-Type": "application/json",
                    headers: {
                        'Authorization': `Bearer ${this.props.token}`
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

    async addCompetitor(competitor) {
        try {
            const response = await axios.post(
                "http://localhost:5000/Competitors/",
                competitor,
                {
                    "Content-Type": "application/json",
                    headers: {
                        'Authorization': `Bearer ${this.props.token}`
                    }
                }
            );


            this.setState({
                ...this.state,
                selectedBtnId: response.data,
            });

            this.initializeCompetitors();
        } catch (err) {
            console.warn(err);
        }
    }

    async editCompetitor(competitor, competitorId) {
        try {
            const response = await axios.put(
                "http://localhost:5000/Competitors?editedCompetitorId=" + competitorId,
                competitor,
                {
                    "Content-Type": "application/json",
                    headers: {
                        'Authorization': `Bearer ${this.props.token}`
                    }
                }
            );

            this.initializeCompetitors()
        } catch (err) {
            console.warn(err);
        }
    }

    async deleteCompetitor(competitorId) {
        try {
            const response = await axios.delete(
                "http://localhost:5000/Competitors?deletedCompetitorId=" + competitorId,
                {
                    headers: {
                        'Authorization': `Bearer ${this.props.token}`
                    }
                }
            );

            if (this.state.selectedBtnId == competitorId) {
                this.setState({
                    ...this.state,
                    selectedBtnId: null,
                });
            }

            this.initializeCompetitors()
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
            addKey: Math.random()
        });
    }

    renderButtons() {
        return this.state.competitors.map((competitor) => {
            let competitorSelected = competitor.id == this.state.selectedBtnId
            return (
                <CompetitorButton
                    id={competitor.id}
                    onClicked={this.onButtonEdit.bind(this)}
                    competitor={competitor}
                    highlight={competitorSelected}
                />
            )
        });
    }

    getEditedCompetitor() {
        const editedCompetitor = this.state.competitors
            .find((competitor) => competitor.id === this.state.selectedBtnId)

        if (editedCompetitor != null) return editedCompetitor

        return {
            alias: '',
            code: '',
            id: this.state.addKey,
            competitorType: 'default'
        }
    }

    render() {
        const competitor = this.getEditedCompetitor()
        const isEditMode = competitor.competitorType === undefined

        let categories = null
        let submitAction = null
        let deleteAction = null

        if (!isEditMode) {
            submitAction = this.addCompetitor
            categories = this.state.categories.map(x => new Object({ value: x.name, label: x.name }))
        } else {
            submitAction = (c) => this.editCompetitor(c, competitor.id)
            deleteAction = (c) => this.deleteCompetitor(competitor.id)
            categories = [{ value: competitor.competition, label: competitor.competition }]
        }

        return (
            <div className="dashboard-container">
                <div className="burger-content">
                    <CompetitorEditView
                        alias={competitor.alias}
                        code={competitor.code}
                        id={competitor.id}
                        key={competitor.id}
                        exception={competitor.exception}
                        submit={submitAction}
                        delete={deleteAction}
                        categories={categories} />

                </div>
                <div className="burger-menu">
                    <div className="title-container">
                        <h1 className="burger-menu-title">Active agents</h1>
                    </div>
                    <div className="buttons-container">
                        {this.renderButtons()}
                    </div>
                    <div className={"competitor-button add-button highlighted-" + !isEditMode}
                        onClick={this.onButtonAdd.bind(this)}>
                        <span className="add-label">Add new agent</span>
                        <div className="plus-sign-container">
                            <span className="plus-sign">+</span>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

}

export default Dashboard;