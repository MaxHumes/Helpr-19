//
//  DetailViewController.swift
//  helperIOShelpr
//
//  Created by Sam Horn on 4/4/20.
//  Copyright © 2020 Sam Horn. All rights reserved.
//

import UIKit
import CoreData

class DetailViewController: UITableViewController, NSFetchedResultsControllerDelegate {

    //@IBOutlet weak var detailDescriptionLabel: UILabel!
    
    @IBOutlet weak var navigator: UINavigationItem!
    //@IBOutlet weak var backButton: UINavigationItem!
    
    
    var thread: Thread?
    var posts: [Post]? = []
    var postView: NewPostView?
  //  var token: Data?
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        let threadButton = UIBarButtonItem(barButtonSystemItem: .compose, target: self, action: #selector(insertNewObject(_:)))
        navigationItem.rightBarButtonItem = threadButton
        
    }
    
    override func viewWillAppear(_ animated: Bool) {
        clearsSelectionOnViewWillAppear = splitViewController!.isCollapsed
        super.viewWillAppear(animated)
        
    }
        
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        let semaphore = DispatchSemaphore (value: 0)
        if thread == nil {
            return
        }
        
        guard let url = URL(string: "https://helpr19api.azurewebsites.net/api/posts/get/\(String(describing: thread?.thread_id))") else { return }
        
        var request = URLRequest(url: url, timeoutInterval: Double.infinity)
        request.httpMethod = "GET"
        request.addValue(String(data: token!, encoding: .utf8)!, forHTTPHeaderField: "token")
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")
    
        let task = URLSession.shared.dataTask(with: request) { data, response, error in
            guard let data = data else {
                
                print(String(describing: error))
                return
            }
        
            guard (response as! HTTPURLResponse?) != nil else {
                print(String(describing: error))
                return
            }
       // print(String(data: data, encoding: .utf8)!)
        //print("\n\n\nresponse: \(String(describing: response))")
            let decoder = JSONDecoder()
        
            do {
                self.posts = try decoder.decode([Post].self, from: data)
                print(String(describing: self.posts))
            } catch {
                print(error.localizedDescription)
            }
            semaphore.signal()
        }
    
        task.resume()
        semaphore.wait()
        var index: Int = 0
        var paths: [IndexPath] = []
        while index < self.posts!.count {
            paths.append(IndexPath(row: index, section: 0))
            index += 1
        }
        self.tableView.insertRows(at: paths, with: .fade)
        task.suspend()
        
    }

    //@IBAction func onPost(_ sender: Any) {
      //  if nameText.text != "" && messageText.text != "" {
            
            //let context = self.fetchedResultsController.managedObjectContext
            //let newPost = Post(context: context)
            
            //newPost.timestamp = Date()
            //newPost.message = postText.text
 
   // }
    
    @objc func insertNewObject(_ sender: Any) {
        performSegue(withIdentifier: "newPost", sender: self)
    }
    
    

  

    //override func viewDidAppear(_ animated: Bool) {
 
    // MARK: - Segues
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        
        if segue.identifier == "newPost" {
            let controller = segue.destination as! NewPostView
            postView = controller
            postView!.thread_id = thread?.thread_id
        }
        //let controller = segue.destination as! NewPostView
        //postView = controller
        //postView!.thread_id = thread?.thread_id
    }
    
    
    // MARK: - Table View
    
    override func numberOfSections(in tableView: UITableView) -> Int {
           return 0

    }
    
    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        //let sectionInfo = fetchedResultsController.sections![section]
        return posts!.count
    }
    
    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let post = (posts?[indexPath.row])!
        let cell = tableView.dequeueReusableCell(withIdentifier: "Post Cell", for: indexPath)

        configureCell(cell, withPost: post)
        return cell
    }

    func configureCell(_ cell: UITableViewCell, withPost post: Post) {
        cell.textLabel!.textColor = UIColor(displayP3Red: 1.0, green: 0.0, blue: 0.0, alpha: 1.0)
        cell.textLabel!.text = String(format: "%s\n", post.name)
       // cell.textLabel!.text = String(format: "%s\n", post.author!.username!)
        cell.textLabel!.textColor = UIColor(displayP3Red: 0.0, green: 0.0, blue: 0.0, alpha: 0.0)
        cell.textLabel!.text = String(format: "%s", post.description)
        //cell.textLabel!.text = String(format: "%s", post.message!)
    }
    
    override func tableView(_ tableView: UITableView, canEditRowAt indexPath: IndexPath) -> Bool {
        // Return false if you do not want the specified item to be editable.
        return true
    }
    

}

