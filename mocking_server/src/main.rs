use std::convert::Infallible;
use std::net::SocketAddr;
use hyper::{Body, Method, Request, Response, Server, StatusCode};
use hyper::service::{make_service_fn, service_fn};

use rand::distributions::{Distribution, Uniform};


#[tokio::main]
async fn main() {
    // We'll bind to 127.0.0.1:3000
    let addr = SocketAddr::from(([127, 0, 0, 1], 8080));

    // A `Service` is needed for every connection, so this
    // creates one from our `hello_world` function.
    let make_svc = make_service_fn(|_conn| async {
        // service_fn converts our function into a `Service`
        Ok::<_, Infallible>(service_fn(router))
    });

    let server = Server::bind(&addr).serve(make_svc);
    let graceful = server.with_graceful_shutdown(shutdown_signal());
    println!("Running server");
    // Run this server for... forever!
    if let Err(e) = graceful.await {
        eprintln!("server error: {}", e);
    }

}

async fn shutdown_signal() {
    // Wait for the CTRL+C signal
    tokio::signal::ctrl_c()
        .await
        .expect("failed to install CTRL+C signal handler");
}


fn method_error() -> Response<Body> {
    let body = Body::from(" Error: The method used is not applicable on this server");
    Response::builder()
        .status(500)
        .header("Content-Type", "text/html")
        .body(body)
        .unwrap()
}


fn handle_request(request: Request<Body>, status: StatusCode) -> Result<Response<Body>, hyper::Error> {

    let mut rng = rand::thread_rng();
    let die = Uniform::from(1..100);
    let throw = die.sample(&mut rng);

    if throw % 3 == 0 {
        Ok(Response::builder()
        .status(StatusCode::SERVICE_UNAVAILABLE)
        .header("Content-Type", "applicaiton/json")
        .body(Body::from("503 Error: Service Unavailable"))
        .unwrap()
        )
    }
    else
    {       
        let message = request.into_body();
        let status_code = status;
        Ok(Response::builder()
        .status(status_code)
        .header("Content-Type", "applicaiton/json")
        .body(message)
        .unwrap()
        )
    }

}

async fn router(req: Request<Body>) -> Result<Response<Body>, hyper::Error> {
    match req.method() {
        // Serve some instructions at /
        // Index page handle
        &Method::GET => handle_request(req, StatusCode::OK),
        &Method::POST => handle_request(req, StatusCode::CREATED),
        &Method::PUT => handle_request(req, StatusCode::NO_CONTENT),
        // Return the 500 on none coded routes.
        _ => {
            Ok(method_error())
        }
    }

}