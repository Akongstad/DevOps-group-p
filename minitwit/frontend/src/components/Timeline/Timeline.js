import * as React from 'react';
import CssBaseline from '@mui/material/CssBaseline';
import Grid from '@mui/material/Grid';
import Container from '@mui/material/Container';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import Header from './Header';
//import Main from './Main';
import Footer from './Footer';
import Message from "./Message";
import {useEffect, useState} from "react";
import PostMessage from './PostMessage';


const sections = [
    /*{ title: 'Technology', url: '#' },
    { title: 'Design', url: '#' },
    { title: 'Culture', url: '#' },
    { title: 'Business', url: '#' },
    { title: 'Politics', url: '#' },
    { title: 'Opinion', url: '#' },
    { title: 'Science', url: '#' },
    { title: 'Health', url: '#' },
    { title: 'Style', url: '#' },
    { title: 'Travel', url: '#' },*/
];
const theme = createTheme();


export default function Timeline() {
    let headers = new Headers();
    headers.append('Content-Type', 'application/json');
    //headers.append('Accept', 'application/json');
    //headers.append('Origin','http://localhost:3000');
    let url = `${window.appConfig.API_URL}/message/timeline`;
    // let url = `${window.appConfig.API_URL}/message/timeline/{username}`;
    
    //let url = `https://minitwit.online/api/msgs`
    //let url = `https://minitwit.online/apiv2/message/timeline`
    //let url = `http://localhost:5229/message/timeline`
    const [posts, setPosts] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    const fetchPost = async () => {
        const response = await fetch(
            url,
        {
            mode: 'cors',
            method: "GET",
            headers: headers
        });
        const data = await response.json();
        setPosts(data);
        setIsLoading(false);
    };
    useEffect(() => {
        fetchPost();
    }, []);
    
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <Container maxWidth="lg">
                <Header title="Minitwit" sections={sections}/>
                <PostMessage/>
                <Grid container spacing={2}  justifyContent={"center"}>
                    { isLoading ? null :
                        posts.map((post) => (
                        <Message key={post.Username} post={post}/>
                        ))
                    }
                    
                </Grid>
            </Container>
            <Footer
                title="TBD Minitwit"
                description="Created for the course: DevOps, Software Evolution and Software Maintenance 2022. IT University of Copenhagen"
            />
        </ThemeProvider>
    );
}
